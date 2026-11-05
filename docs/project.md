# Project Structure & Tuning

## Folder layout

```
2d-platformer-basics/
├── GlobalScripts/Main.cs          ← Main node: wires enemy death signals
├── Levels/level.tscn              ← the level
├── Enemy/
│   ├── enemy.tscn
│   ├── scripts/Enemy.cs
│   └── sprites/                   ← 6 monster sprite sets (1 used)
├── player/
│   ├── player.tscn
│   ├── scripts/
│   │   ├── Player.cs
│   │   └── state_machine/
│   │       ├── PlayerState.cs           ← base state
│   │       ├── PlayerStateMachine.cs    ← controller
│   │       └── states/
│   │           ├── PlayerIdleState.cs
│   │           ├── PlayerRunState.cs
│   │           ├── PlayerJumpState.cs
│   │           ├── PlayerFallState.cs
│   │           ├── PlayerCrouchState.cs
│   │           └── PlayerDieState.cs
│   ├── audio/                     ← jump.wav, skid.wav, die.mp3
│   └── sprites/marlo.png
├── assets/
│   ├── tileset/                   ← level_01.tres, pngs
│   ├── images/                    ← Pink Man, backgrounds, Snail, Tilemap, Collectibles
│   └── sounds/                    ← music.ogg, collect_apple.wav, death.wav (unused)
├── project.godot
├── 2D Platformer Basics.csproj / .sln   ← .NET project (net8.0)
├── icon.svg
└── docs/                          ← this documentation
```

Namespaces: `PlayerNode` (`Player.cs`), `EnemyNode` (`Enemy.cs`),
`PlayerNode.StateMachine`, `PlayerNode.StateMachine.States`.

## Physics layers

Named in `project.godot` (`[layer_names]`):

| Layer | Name | Used by |
|---|---|---|
| 1 | Player | Player body |
| 5 | Terrain | Tiles / terrain tileset, enemy instances (`mask = 16`) |
| 6 | One Way Platform | tileset one-way platforms |

- Player: `collision_mask = 49` (layers 1, 5, 6) — stands on terrain and
  one-way platforms.
- Enemy: `collision_layer = 0`, `collision_mask = 16` — walks on terrain,
  passes through everything else; kills via its `Area2D` (layer 0) detecting
  the player.
- Crouch `RayCast2D`: `collision_mask = 32` (layer 6) — checks for a ceiling
  made of one-way platforms before crouch-jumping.

> The enemy kill check is by **node name** (`body.Name == "Player"`), not by
> physics layer or group — keep the player node named `Player`.

## Tuning the feel

Most gameplay numbers are **exported properties**, mostly set as overrides in
`player.tscn` (Run, Jump nodes). Change them in the Inspector, no code needed:

| Setting | Where | Notes |
|---|---|---|
| `Speed` / `SprintSpeed` | `Run` node | `100` / `350` px/s |
| `Acceleration` / `SkidAcceleration` | `Run` node | `4` / `8` |
| `JumpVelocity` | `Jump` node | `400` (script default `350`) |
| `Acceleration` (air) | `Jump` node | `5` (script default `8`) |
| `BaseMovementSpeed` | `Jump` / `Fall` nodes | `100` |
| `FallGravityMultiplier` | `Fall` node | `1.165` — heavier fall |
| `CoyoteTime` | `Fall` node | `0.125 s` |
| `Deceleration` | `Idle` / `Crouch` nodes | `8` / `4` |
| `FreezeTime` / `PopForce` / `TotalTime` | `Die` node | `0.4 s` / `-350` / `2 s` |
| `Speed` / `Direction` | `Enemy` instances | `25` px/s, `-1` (left) |
| `wait_time` | enemy `Timer` | `2 s` — enemy turn interval |

## Adding things

- **New player state** — create a `PlayerState` subclass under
  `player/scripts/state_machine/states/`, add it as a child of
  `PlayerStateMachine` in `player.tscn` with `unique_name_in_owner = true`,
  and give it a `%Name` reference in `PlayerState._Ready()`. The machine
  picks up all `PlayerState` children automatically in `Init()`.
- **New enemy type** — duplicate `enemy.tscn`, change sprites/script, add it
  under `Enemies` in `level.tscn`. `Main.SetupLevel()` auto-subscribes to
  `PlayerDied` for every `Enemy` child, so new enemies must emit that signal.
- **New level** — duplicate `Levels/level.tscn`, or instance it from a new
  scene and repoint the `Level` export on `main.tscn`. New levels need an
  `Enemies` node for the death wiring to work.
- **New input action** — add it in Project Settings → Input Map (or
  `project.godot` `[input]`), then read it in the relevant state.

## Building / exporting

It's a standard Godot 4.7 .NET project: open the folder in Godot with .NET
support, build (`2D Platformer Basics.sln`, `net8.0`), and export through
Project → Export. No `export_presets.cfg` is checked in, so you'll need to
create export presets (and install export templates) first.
