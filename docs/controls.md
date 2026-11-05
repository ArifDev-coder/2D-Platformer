# Controls & Input

Input actions are defined in `project.godot` (`[input]`) and read either by
`PlayerStateMachine._Process` (as a shared `Direction` vector) or by individual
states via `_UnhandledInput`. Deadzone is `0.2` for all actions.

| Action | Keys | Read by | Effect |
|---|---|---|---|
| `left` | `A`, `←` (Left arrow) | Direction vector | Move left |
| `right` | `D`, `→` (Right arrow) | Direction vector | Move right |
| `up` | `W`, `↑` | Direction vector | Up (`Direction.Y < 0`) |
| `down` | `S`, `↓` | Direction vector | Crouch (in `Idle`/`Run`) |
| `jump` | `Space`, `X` | States | Jump; release for variable height; coyote jump in `Fall` |
| `action` | `Enter` | `RunState` | Sprint while held |

> `action` is bound to a single key (`KEY_ENTER`, physical keycode `4194325`).
> There is no sprint key on the number row or a controller — if you want
> `Shift`, add it to the `action` action in Project Settings → Input Map.

## How input flows

1. `PlayerStateMachine._Process` polls `Input.GetAxis("left", "right")` and
   `Input.GetAxis("up", "down")` into the static `PlayerState.Direction`.
   Every state reads `Direction` instead of polling input itself.
2. Discrete events (`jump` press/release, `action` press/release) are routed
   to the **current state only** via `_UnhandledInput` →
   `CurrentState.HandleInput(@event)`, which may return a new state.
3. Physics outcomes (`IsOnFloor`, `Velocity`) drive transitions in
   `PhysicsProcess` — e.g. `RunState` → `Fall` when walking off a ledge.

## Detailed behaviours

- **Variable jump height** — releasing `jump` mid-rise in `JumpState` halves
  `Velocity.Y` (`* .5f`) and immediately transitions to `Fall`.
- **Coyote time** — in `FallState`, `jump` within `0.125 s` of leaving a ledge
  still triggers `Jump`. Coyote time is disabled if the previous state was
  `Jump` (prevents infinite re-jumps).
- **Sprint** — holding `action` in `RunState` switches `TargetSpeed` from
  `100 px/s` to `350 px/s` (override in `player.tscn`; script default is
  `150 px/s` — see [Player](player.md)). Animation `SpeedScale` tracks
  actual speed.
- **Skid** — reversing direction in `RunState` plays the `skid` animation
  with `SkidAcceleration = 8` and plays `skid.wav`.
- **Crouch** — holding `down` from `Idle`/`Run` enters `Crouch`, which swaps
  in the shorter `CollisionShape2D_Crouch`. Jumping while crouched under a
  low ceiling (`RayCast2D` detects collision) drops you into `Fall` instead.
