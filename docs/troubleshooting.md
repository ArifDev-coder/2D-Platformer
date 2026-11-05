# Troubleshooting & Roadmap

## Known gaps

These are things that exist in the scene/assets but aren't functional yet.
They're listed here so future work doesn't have to rediscover them.

<a id="known-gaps"></a>
### `Exit` area does nothing

`Levels/level.tscn` has an `Area2D` named `Exit` at the goal, but no script and
no `body_entered` connection — crossing the finish line has no effect.

**Fix:** add a small script on the `Exit` node:

```csharp
// Goal.cs
using Godot;

public partial class Goal : Area2D
{
    public override void _Ready()
    {
        BodyEntered += (body) =>
        {
            if (body.Name == "Player")
            {
                GetTree().ChangeSceneToFile("res://Levels/level2.tscn"); // or win logic
            }
        };
    }
}
```

### No health, score, or checkpoints

Touching an enemy = instant death → `ReloadCurrentScene()`. The checkpoint
flag asset (`assets/images/Tilemap/Checkpoint (Flag Idle)(64x64).png`) is
placed/hidden in the level but has no logic.

### Void floor is a safe floor, not a kill zone

`BoundaryLimits/Void` is a `WorldBoundaryShape2D` static body — falling off
the world lands you on an invisible floor at `y ≈ 278` (approximate) instead of
dying. Replace it with an `Area2D` that calls `Player.PlayerDie()` to make
pits lethal.

### Enemy movement is timer-based only

Enemies reverse direction every `2 s` regardless of walls or ledges. They
will walk through walls (they only collide with Terrain, and nothing stops
them) and off edges. Use `IsOnWall()` / a floor-edge raycast in
`Enemy._PhysicsProcess` for patrol logic.

## Unused assets & roadmap

The repo ships several complete asset sets that aren't wired to any scene —
likely the intended roadmap:

| Asset | Status | Suggested use |
|---|---|---|
| `assets/sounds/music.ogg` | unused | Background music `AudioStreamPlayer` |
| `assets/sounds/collect_apple.wav` + `Collectibles/*.png` | unused | Apples + score system |
| `assets/sounds/death.wav` | unused | Alternative death sound |
| `assets/images/Pink Man/*.png` | unused | Second character / enemy palette |
| `Enemy/sprites/2 - Lil Wiz`, `5 - Moe Scotty`, `4 - Squirmy Wormy`, `7 - Grizzly`, `8 - Orc` | unused | New enemy types (caster, flyer, …) |
| `assets/images/Snail/*.png` | unused | Turtle enemy with shell-hit states |
| `assets/images/Background/{Blue,Brown,Gray,Green}.png`, `fonts/Square-Black.ttf` | unused | Level backgrounds, UI font |
| `Doggo`/double-jump/wall-jump frames in Pink Man set | unused | Double jump, wall jump mechanics |

## Gotchas

- **Enemies detect the player by name.** `OnBodyEntered` checks
  `body.Name == "Player"`. Renaming the player node breaks enemy kills.
- **Player spawns crouched.** `PlayerStateMachine.Init` enters `Crouch` first;
  the player stands only when `Direction.Y <= 0` (i.e. `down` released).
- **`action` (sprint) is `Enter` only.** Sprint can't be toggled with Shift
  unless you add it — see [Controls](controls.md).
- **Script defaults vs scene overrides differ.** E.g. `PlayerRunState` code
  says `SprintSpeed = 150` but `player.tscn` overrides to `350`; `PlayerJumpState`
  says `JumpVelocity = 350` but the scene sets `400`. Always check `player.tscn`
  for the live values.
- **Godot version / .NET required.** This project needs Godot 4.7 with .NET
  support; opening it in a non-.NET build will fail to load the C# scripts.
