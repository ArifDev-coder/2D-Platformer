# Player & State Machine

The player is a `CharacterBody2D` (`player/player.tscn`) whose behaviour is
entirely driven by a **6-state state machine**. Physics lives in `Player.cs`;
decisions live in the states.

```
player/player.tscn  (CharacterBody2D, collision_mask = 49)
├── Camera2D
├── Sprite2D                     ← marlo.png, hframes = 8
│   └── AnimationPlayer          ← idle / run / skid / jump / fall / crouch
├── AudioStreamPlayer2D          ← shared by all states
├── CollisionShape2D             ← standing hitbox, 11.5 × 22 px
├── CollisionShape2D_Crouch      ← crouch hitbox, 11.5 × 13 px (disabled by default)
└── PlayerStateMachine (Node2D)
    ├── Idle    (PlayerIdleState)
    ├── Run     (PlayerRunState)
    ├── Jump    (PlayerJumpState)
    ├── Fall    (PlayerFallState)
    ├── Crouch  (PlayerCrouchState)
    │   └── RayCast2D            ← ceiling check for crouch-jump
    └── Die     (PlayerDieState)
```

## Player.cs — the body

`player/scripts/Player.cs` (`namespace PlayerNode : CharacterBody2D`):

- `GravityMultiplier` (default `1`) — scaled gravity; `FallState` raises it.
- `_Ready()` grabs `Sprite2D`, `Sprite2D/AnimationPlayer`,
  `AudioStreamPlayer2D`, and `PlayerStateMachine`, then calls
  `PlayerStateMachine.Init(this)`.
- `_PhysicsProcess(delta)` applies gravity when airborne, then `MoveAndSlide()`.
  > Comment in code: *"Add !IsOnFloor() to fix slop glitch"* — the check is
  > intentional (avoids accumulating gravity on the floor).
- `UpdateVelocity(target, acceleration)` — `Mathf.MoveToward` on `Velocity.X`.
- `UpdateDirection(dir)` — flips `Sprite.Scale.X` to `±1`.
- `PlayAudio(stream)` — null-safe one-shot via the shared player.
- `PlayerDie()` — requests the `Die` state from the state machine.

## PlayerStateMachine.cs — the controller

`player/scripts/state_machine/PlayerStateMachine.cs`:

- Keeps a stack (max `StateSize = 3`) of visited states.
  `CurrentState = States[0]`, `PreviousState = States[1]`.
- The machine starts with `ProcessMode = Disabled`. `Init(player)` collects
  every `PlayerState` child node, wires static references
  `PlayerState.Player` / `PlayerState.StateMachine`, calls `Init()` on each
  state, enters **`Crouch`** (the machine's first `ChangeState` call — the
  player spawns in a crouch until `Direction.Y <= 0` → `Idle`),
  then sets `ProcessMode = Inherit` so processing begins.
- Each frame it polls input → `Direction`, then **forwards** the same tick to
  the current state three ways, applying any returned state immediately:
  - `_Process` → `CurrentState.Process(delta)`
  - `_PhysicsProcess` → `CurrentState.PhysicsProcess(delta)`
  - `_UnhandledInput` → `CurrentState.HandleInput(@event)`
- `ChangeState(newState)` is a no-op for `null` (stay) or the current state;
  otherwise it calls `CurrentState.Exit()`, pushes the new state, `Enter()`s
  it, and resizes the stack to 3.
- **States are the scene's child nodes**, not code-created objects. Every
  `PlayerState._Ready()` resolves the other states by unique name
  (`%Idle`, `%Run`, `%Jump`, `%Fall`, `%Crouch`, `%Die`) — so all states
  know each other's instances and can return them directly.

## State transition diagram

```
              jump
  ┌──────┐  ──────▶  ┌──────┐  jump released / Velocity.Y ≥ 0   ┌──────┐
  │ Idle │            │ Jump │ ─────────────────────────────────▶ │ Fall │
  └─┬──┬─┘            └──┬───┘                                  └───┬──┘
    │  │                 │  coyote jump (Fall → Jump)               │ land (IsOnFloor)
    │  │ direction       └──────────────────────────────────────────┘
    │  └────▶ Run ───────┘
    │            (land: Jump/Fall → Idle)
    │
    └── down ──▶ Crouch ── up ──▶ Idle ──┘
                    │ jump: RayCast hit → Fall / else → Jump
                    └ !IsOnFloor → Fall

  any state ──(enemy touch)──▶ Die ──2 s──▶ ReloadCurrentScene
```

## State details & tunables

### Idle — `PlayerIdleState`

- `Enter`: plays `idle`.
- `PhysicsProcess`: decelerates to 0 (`Deceleration = 8`).
  Transitions: `Direction.X ≠ 0` → `Run`; `Direction.Y > 0` → `Crouch`;
  `!IsOnFloor()` → `Fall`.
- `HandleInput`: `jump` → `Jump`.

### Run — `PlayerRunState`

- Exports (script defaults / `player.tscn` overrides):
  `Speed = 100 px/s`, `SprintSpeed = 350 px/s` *(override; code default 150)*,
  `Acceleration = 4`, `SkidAcceleration = 8`, `SkidAudio = skid.wav`.
- `Enter`: plays `run`, sets `TargetSpeed` to `SprintSpeed` if `action` is
  already held; subscribes to `AnimPlayer.CurrentAnimationChanged`.
- `PhysicsProcess`: `!IsOnFloor()` → `Fall`; `Direction.X == 0` → `Idle`;
  `Direction.Y > 0` → `Crouch`. If input sign ≠ velocity sign → *skid*
  (`skid` anim + `SkidAcceleration` + skid audio via the animation callback),
  else normal run (`run` anim, `SpeedScale = |Velocity.X| / Speed`).
- `HandleInput`: `action` press/release toggles sprint target; `jump` → `Jump`.
- `Exit`: unsubscribes, stops audio, resets `SpeedScale = 1`.

### Jump — `PlayerJumpState`

- Exports: `JumpVelocity = 400` *(override; code 350)*, `BaseMovementSpeed = 100`,
  `Acceleration = 5` *(override; code 8)*, `JumpAudio = jump.wav`.
- `Enter`: plays `jump`, nudges position up 1 px, sets
  `Velocity = (Velocity.X, -JumpVelocity)`, preserves horizontal speed ≥ base
  speed (`MoveSpeed = Max(BaseMovementSpeed, |Velocity.X|)`), plays jump audio.
- `HandleInput`: `jump` released → halves upward velocity, → `Fall`.
- `PhysicsProcess`: applies air control; `IsOnFloor()` → `Idle`,
  `Velocity.Y ≥ 0` → `Fall`.

### Fall — `PlayerFallState`

- Exports: `BaseMovementSpeed = 100`, `FallGravityMultiplier = 1.165`,
  `Acceleration = 8`, `CoyoteTime = 0.125 s`.
- `Enter`: plays `fall`, snapshots `MoveSpeed`, starts coyote timer,
  raises `Player.GravityMultiplier`. If previous state was `Jump`, coyote
  timer is `0` (no mid-air re-jump).
- `HandleInput`: `jump` while `CoyoteTimer > 0` → `Jump`.
- `PhysicsProcess`: decrements coyote timer, applies air control;
  `IsOnFloor()` → `Idle`. `Exit` resets `GravityMultiplier = 1`.

### Crouch — `PlayerCrouchState`

- `Deceleration = 4`. `Init` caches the two hitboxes (paths
  `../../CollisionShape2D`, `../../CollisionShape2D_Crouch`) and its
  `RayCast2D` (child of the state node); raycast disabled until crouching.
- `Enter`/`Exit` swap `CollisionShape2D.Disabled` ↔
  `CollisionShape2D_Crouch.Disabled` and enable/disable the `RayCast2D`.
- `PhysicsProcess`: decelerates; `Direction.Y <= 0` → `Idle`;
  `!IsOnFloor()` → `Fall`.
- `HandleInput`: `jump` → if `RayCast2D.IsColliding()` (ceiling), shift down
  2 px and → `Fall`; else → `Jump`.

### Die — `PlayerDieState`

- Exports: `FreezeTime = 0.4 s`, `PopForce = -350`, `TotalTime = 2 s`,
  `DieAudio = die.mp3`.
- `Enter`: zero velocity, play die audio (+10 dB), play & pause the `fall`
  animation (freeze-frame), disable the standing hitbox (deferred),
  detach `Camera2D` to the scene root (smoothing off), raise `ZIndex = 100`.
- `PhysicsProcess`: after `FreezeTime`, pop upward with `PopForce`, then
  apply raw gravity; after `TotalTime`, `ReloadCurrentScene()`.
  No input is handled in this state.

## Animation library

The `AnimationPlayer` under `Sprite2D` animates `frame` on a sprite sheet with
8 horizontal frames (`marlo.png`):

| Animation | Frame(s) | Notes |
|---|---|---|
| `idle` | 0 | autoplay |
| `run` | 1 → 2 → 1 → 0, 0.1 s steps (0.4 s loop) | speed-scaled by `RunState` |
| `skid` | 3 | single frame |
| `jump` | 4 | |
| `fall` | 5 | also used (frozen) for death |
| `crouch` | 6 | |
| `RESET` | 0 | default |

There's no separate "sprint" animation — running faster just plays `run`
faster via `SpeedScale`.
