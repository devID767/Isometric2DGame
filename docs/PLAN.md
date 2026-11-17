# Plan

![Workflow](workflow.png)

Core features first. Stretch goals start only after all four tasks are merged into `main`.
Estimates are focused work hours, breaks not included.

## Estimates

| Block | Estimate |
|---|---|
| Planning | 1.5 h |
| Asset sourcing | 2.5 h |
| Task 1 - Project setup | 2 h |
| Task 2 - Movement and camera | 4 h |
| Task 3 - Enemy AI | 5 h |
| Task 4 - Combat | 5 h |
| Polish and QA | 4 h |
| Submission | 1 h |
| **Core total** | **25 h** |
| Stretch goals | ~5 h |

## Tasks

**Planning**
- Read the brief, note open questions
- Workflow diagram and estimates
- Send the plan and repository link

**Asset sourcing** (runs alongside Tasks 2-3, placeholders until it is done)
- Free 2D sprites close to the project style
- License check, credits in README
- Import settings and slicing

**Task 1 - Project setup**
- Unity 6000.4.1f1, 2D URP template
- Git, Unity `.gitignore`, `.gitattributes`
- Public GitHub repository, empty project pushed
- README skeleton

**Task 2 - Movement and camera**
- New Input System, Move action for WASD and arrow keys
- Dynamic Rigidbody2D movement with configurable max speed and instant acceleration
- Player collider
- 3/4 top-down Tilemap with Y-sorting and test obstacles
- Cinemachine follow camera
- Direction flip or walk animation

**Task 3 - Enemy AI**
- State machine: Idle, Patrol, Chase, Attack
- Player detection with LayerMasks and line of sight
- Random patrol points
- Health component shared with the player
- State shown by colour, debug gizmos in the editor

**Task 4 - Combat**
- Melee attack on Space / left mouse button
- Auto-firing projectile weapon
- Player and enemy health
- Health UI
- Hit flash, knockback, camera shake
- Death, Game Over screen, restart

**Polish and QA**
- Level layout and enemy placement
- Enemy animations
- UI styling
- Playtest as a player, fix what feels wrong
- Code cleanup and comments

**Submission**
- README: how to test, controls, decisions, credits, AI usage
- Estimate vs actual
- Send the final repository

## Stretch goals (in this order)

1. Hit particles - 1 h
2. Attack and death animations - 1 h
3. 2D lighting and post-processing - 1 h
4. NavMesh pathfinding for enemies - 2 h

## Decisions

Added as they are made.

- **View: 3/4 top-down instead of a strict isometric grid** (17.11). I asked the team which one they expect, the answer was that either is fine. I chose 3/4 because Nanokings uses it and because all art can then come from one author with the same pixel density and palette.
- **W moves the player up the screen.** With a 3/4 view the grid matches the screen axes, so input goes to the motor without conversion.
- **Art: Cole Foster packs (CC0)** for characters, enemies, props and tiles. No free kit matched Nanokings closely, so I picked one consistent CC0 source over mixing authors. The sprites are rendered by a generator whose code was written with AI coding agents. This is stated in the credits.
- **No Unity Asset Store art.** The standard Asset Store EULA does not allow putting the files in a public repository, and reviewers need to open the project as is.
- **Instant acceleration by default.** `CharacterMoveConfig` has max speed, an instant acceleration toggle and an acceleration value for a smooth ramp, so both readings of the requirement are covered.
- **Movement logic in plain C# classes.** `CharacterMotor` and `PlayerInputReader` are not MonoBehaviours. `Player` creates them and forwards Unity callbacks (Humble Object). Enemies will reuse the same motor.
- **Camera: Cinemachine 3 follow camera, no custom script.** Rigidbody interpolation is on to avoid jitter.
