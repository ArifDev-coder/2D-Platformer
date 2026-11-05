# Enemies

The game currently uses a single enemy type: a **patrolling crawler**
("Mr. Chomps" sprites, `Enemy/sprites/6 - Mr. Chomps/`). It's instanced
**16 times** in `level.tscn` under the `Enemies` node.

## Scene structure — `Enemy/enemy.tscn`

```
Enemy (CharacterBody2D, Speed = 25)
├── Sprite (Sprite2D, scale -1,1)
│   ├── Walk (Sprite2D)          ← Crawling_(32x32).png, 6 hframes (hidden by default)
│   └── Idle (Sprite2D)          ← Eye_Blink…png
├── AnimationPlayer              ← "Walk" animation (autoplay) toggles Walk/Idle visibility
├── Timer (wait_time = 2 s, autostart)
├── CollisionShape2D             ← 25.5 × 11.5 px body
├── CollisionShape2D2            ← 20.25 × 5.5 px top bump
└── Area2D (collision_layer = 0)
    ├── CollisionShape2D         ← kill zone, mirrors the body
    └── CollisionShape2D2
```

In `level.tscn` every instance overrides `collision_layer = 0` and
`collision_mask = 16` (layer 5, "Terrain") so enemies walk on terrain but
ignore each other and the player physically.

## Behaviour — `Enemy/scripts/Enemy.cs`

`namespace EnemyNode : CharacterBody2D`

- Exports: `Speed = 25 px/s`, `Direction = -1` (starts walking left).
- Signal: `PlayerDied(Node2D body)`.
- `_PhysicsProcess`: applies gravity when airborne, then
  `Velocity = (Direction * Speed, Velocity.Y)` and `MoveAndSlide()`.
- `Timer.timeout` → `_on_timer_timeout()`: flips `Direction *= -1` and flips
  the `Sprite.Scale.X`. **The enemy reverses direction purely on a 2-second
  timer — not on walls or ledges.** Wall/edge-awareness is not implemented.
  > The commented-out code in `_on_timer_timeout` shows `Walk`/`Walk_flip`
  > animations were planned for the flip, but the current sprite flip is done
  > by scaling instead.
- `Area2D.BodyEntered` → `OnBodyEntered`: if `body.Name == "Player"`, emits
  `PlayerDied(body)`. Detection is **by node name**, not by group or layer.

## Kill flow

```
Enemy.OnBodyEntered("Player")
  → Enemy emits "PlayerDied"
  → Main.OnPlayerDied (Main.cs, subscribed per-enemy in SetupLevel)
  → Player.PlayerDie()
  → PlayerStateMachine.ChangeState(Die)
  → PlayerDieState: freeze → pop → ReloadCurrentScene after 2 s
```

See [Gameplay](gameplay.md#death--respawn). The `Area2D` detects the player even
though the enemy's body only collides with terrain — contact with any part of
the overlapping kill zone kills the player.

## Unused enemy assets

`Enemy/sprites/` contains complete sprite sets that are **not wired to any
scene**: Lil Wiz (mage), Moe Scotty (flyer), Squirmy Wormy, Grizzly, Orc, and
the Snail set under `assets/images/Snail`. These are presumably material for
future enemy types. See [Troubleshooting](troubleshooting.md#unused-assets--roadmap).
