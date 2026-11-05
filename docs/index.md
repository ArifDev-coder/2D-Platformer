# 2D Platformer Basics — Documentation

A 2D platformer built with **Godot 4.7 (C# / .NET 8)**. It implements a classic
platformer controller with a **state-machine-driven player**, patrol enemies,
coyote time, variable jump height, sprint, skid, and crouch.

| Item | Value |
|---|---|
| Engine | Godot 4.7, C# (`Godot.NET.Sdk/4.7.2`) |
| Target framework | `net8.0` (`net9.0` on Android) |
| Rendering | Forward Plus (Windows: D3D12) |
| Physics (3D) | Jolt Physics |
| Resolution / stretch | 320×180, `canvas_items` / `expand` (1280×720 override) |
| Main scene | `main.tscn` → `Levels/level.tscn` |
| Player character | "Marlo" (`player/sprites/marlo.png`) |

## Quick start

1. Open this folder in **Godot 4.7+ with .NET support**.
2. Run `main.tscn` (F6 / F5). The game starts directly in `Level`.
3. Controls: `A/D` or `←/→` move, `Space`/`X` jump, `Shift`/`Enter` sprint,
   `S`/`↓` crouch. See [Controls & Input](controls.md).
4. Reach the flag on the far right of the level.

## Contents

- [Controls & Input](controls.md) — every action and key binding.
- [Gameplay Overview](gameplay.md) — how a run plays, death/respawn, level flow.
- [Player & State Machine](player.md) — `Player`, `PlayerStateMachine`,
  and all 6 states in detail (transitions, parameters, tuning).
- [Enemies](enemies.md) — patrol enemy behaviour, collision, signal flow.
- [Levels & Scenes](levels.md) — scene tree, TileMap layers, boundaries, the `Exit` area.
- [Project Structure & Tuning](project.md) — folders, physics layers, how to
  tweak speeds/jumps and add states/enemies/levels.
- [Troubleshooting & Roadmap](troubleshooting.md) — known quirks, limits, and
  what the unused assets suggest about future work.
