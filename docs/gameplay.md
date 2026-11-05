# Gameplay Overview

## Goal

Run from the spawn on the left (`Player` at `(-664, -23)` in `level.tscn`)
to the flag at the far right end of the level (~`x = 2676`) while avoiding
patrolling enemies (16 of them under the `Enemies` node).

## Core loop

1. **Move** — run left/right, use one-way platforms and terrain tiles to go
   up and across.
2. **Jump** — hop gaps and climb; short-hop by tapping `jump`, full-hop by
   holding it. You can also jump shortly after walking off an edge
   (coyote time).
3. **Sprint / skid** — hold `action` to sprint; reversing abruptly skids.
4. **Crouch** — use `down` for low ceilings (the crouch hitbox is shorter).
5. **Avoid enemies** — touching an enemy's `Area2D` kills the player
   (see [Enemies](enemies.md)). There is currently **no way to kill or stomp
   enemies**.
6. **Finish** — reach the `Exit` area at the right end. ⚠️ The `Exit` area
   exists in the scene but has **no connected script/signal yet** — see
   [Troubleshooting](troubleshooting.md#known-gaps).

## Death & respawn

When the player touches an enemy:

```
Enemy.OnBodyEntered("Player")
  → Enemy emits Signal "PlayerDied"          (Enemy.cs)
  → Main.OnPlayerDied                        (Main.cs)
  → Player.PlayerDie()                       (Player.cs)
  → PlayerStateMachine.ChangeState(Die)      (PlayerStateMachine.cs)
  → PlayerDieState.Enter / PhysicsProcess    (PlayerDieState.cs)
```

`PlayerDieState` plays a freeze-frame for `FreezeTime = 0.4 s` (velocity zeroed,
animation paused), then pops the player upward (`PopForce = -350`) with
gravity, and after `TotalTime = 2 s` calls `GetTree().ReloadCurrentScene()` —
the whole level restarts. The `Camera2D` is reparented to the scene root so it
stays still while the body pops.

> There is **no health/lives system** and **no checkpoint respawn** — death
> always reloads the current scene. The `Checkpoint (Flag Idle)(64x64).png`
> asset suggests checkpoints were planned but aren't implemented.

## Level flow

- `main.tscn` (root `Main : Node2D`) instances `Levels/level.tscn`.
- `Main._Ready()` finds `Level/Enemies`, subscribes to every `Enemy.PlayerDied`
  signal, and forwards the kill to the player. If `Enemies` is missing, it
  silently does nothing (null-checked).
- The `Level` scene contains: `Terrain` (4 `TileMapLayer`s), `Player`, `Sky`
  (parallax gradient), `Enemies` (16× `Enemy`), `Flag` sprite, `Exit` area,
  and `BoundaryLimits` (static walls + void floor).
- World boundaries: `BoundaryLimits/Left` and `Right` are `WorldBoundaryShape2D`
  static bodies so the player can't leave the level; `Void` is a floor at
  `y ≈ 278` (approximate) to catch falls.

## Audio

One `AudioStreamPlayer2D` on the player, reused by all states:

| Sound | When | Volume |
|---|---|---|
| `jump.wav` (in `player/audio`) | `JumpState.Enter` | `-10 dB` |
| `skid.wav` | `RunState` enters skid animation | `0 dB` |
| `die.mp3` | `DieState.Enter` | `+10 dB` |

See the `assets/sounds` folder (`music.ogg`, `collect_apple.wav`, `death.wav`)
for **unused** sounds — no music player or collectible system is wired up yet.
