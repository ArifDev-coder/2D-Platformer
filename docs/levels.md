# Levels & Scenes

## Scene graph

```
main.tscn                          (Main : Node2D, Main.cs)
└── Level  ← Levels/level.tscn
    ├── Terrain (Node2D)
    │   ├── TileMapLayer2          ← background-ish layer
    │   ├── TileMapLayer           ← main gameplay tiles
    │   ├── TileMapBackground      ← z_index = -1
    │   └── TileMapBackground2     ← z_index = -1
    ├── Player  ← player/player.tscn  (spawn at -664, -23)
    ├── Sky (Parallax2D, scroll_scale 0,0)
    │   └── Sprite2D               ← 1024×512 gradient (hidden)
    ├── Enemies (Node2D)           ← 16× Enemy (enemy.tscn)
    ├── Flag (Sprite2D)            ← 10 hframes, "flag" anim (hidden)
    ├── Exit (Area2D)              ← tall trigger at the right end
    └── BoundaryLimits (Node2D)
        ├── Left  (StaticBody2D + WorldBoundaryShape2D)
        ├── Right (StaticBody2D + WorldBoundaryShape2D)
        └── Void  (StaticBody2D + WorldBoundaryShape2D)
```

## Files

| File | Role |
|---|---|
| `main.tscn` | Root scene (launch point). One `Main` node + one instanced `Level`. |
| `Levels/level.tscn` | The only level. Tiles, player, enemies, sky, boundaries. |
| `player/player.tscn` | The player CharacterBody2D + state machine. |
| `Enemy/enemy.tscn` | The single patrol-enemy type. |

## Terrain & tiles

All four `TileMapLayer`s share one `TileSet` resource:
`assets/tileset/level_01.tres` (built from `tileset.png` + `cloud-01/02.png` +
`background.png` + `Terrain (16x16).png`). The theme is a pixel-art
terrain/sky set; the `TileMapBackground` layers sit at `z_index = -1` behind
the gameplay layer. Terrain tiles are on physics layer 5 ("Terrain") — see
[Project Structure](project.md#physics-layers).

## Sky & background

`Sky` is a `Parallax2D` with `scroll_scale = (0, 0)` and a single hidden
`Sprite2D` using a `GradientTexture2D` (blue gradient, 1024×512). Because
the scroll scale is zero, it acts as a fixed backdrop. The `Flag` sprite at
the goal is also hidden; its `AnimationPlayer` autoplays a 2-frame `flag`
animation (frames 0 → 9, 1 s, loop).

## Player spawn

The `Player` instance in the level is placed at `(-664, -23)`, near the
left edge. The state machine starts in `Crouch` (see
[Player](player.md#playerstatemachinecs--the-controller)), so the player
begins crouched and stands when `down` is released.

## Exit (goal)

`Exit` is an `Area2D` at `(2730, -31)` with a tall `RectangleShape2D`
(55 × 1114 px) — essentially a finish-line trigger wall at the right end of
the level. ⚠️ **It has no script and no `body_entered` connection**, so
walking into it currently does nothing. See
[Troubleshooting](troubleshooting.md#known-gaps) for how to wire it up.

## Boundaries

`BoundaryLimits` uses three `StaticBody2D`s with `WorldBoundaryShape2D`
infinite planes:

- `Left` — blocks the player from leaving on the left (~`x = -864`).
- `Right` — blocks on the right (`x ≈ 2811` after transforms).
- `Void` — a floor plane at `y ≈ 278` (approximate) that catches the player
  if they fall below the level. Note: it is a plain static body, **not a kill
  zone** — falling into the void lands on an invisible floor rather than
  killing the player.
