# Isometric2DGame

Isometric 2D prototype made for the Runic Dices Entertainment programming trial.
Specialization: Combat.

## Requirements
- Unity 6000.4.1f1
- No extra setup, packages restore on first open.

## How to run
1. Clone the repository.
2. Open the folder in Unity Hub (Add > Add project from disk) with 6000.4.1f1.
3. Open `Assets/_Project/Scenes/Game.unity` and press Play.

## Controls
- Move: WASD or arrow keys
- TODO

## How to test
TODO: what to check for each task.
- Task 2, movement:
- Task 3, enemy AI:
- Task 4, combat:

## Project structure
TODO

## Decisions
- View: 3/4 top-down instead of a strict isometric grid. I asked the team, both were accepted. It matches Nanokings and keeps all art from one author.
- W moves the player up the screen, S down, A left, D right.
- Acceleration is instant by default. A smooth ramp can be enabled in `CharacterMoveConfig`, where max speed is also set.
- Camera: Cinemachine follow camera, no custom script.

More details in [docs/PLAN.md](docs/PLAN.md#decisions).

## Assets
- [Voxel Dungeon Pack](https://colefoster.itch.io/voxel-dungeon-pack) and [Voxel Crypt Pack](https://colefoster.itch.io/voxel-crypt-pack) by Cole Foster, CC0 1.0. Characters, props, effects and tiles. License files are in `Assets/_Project/Art/ThirdParty/ColeFoster/`. According to the pack README, the sprites are rendered by a generator whose code was written with AI coding agents; no image-generation model was used.

## AI usage
I discussed architecture with AI and used it for research, code review and git housekeeping. All code was written by me.
