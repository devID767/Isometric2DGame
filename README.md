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
- Move: WASD, arrow keys or gamepad left stick

## How to test
Open `Assets/_Project/Scenes/Game.unity` and press Play.

### Task 2, movement and camera
- **Movement.** WASD or arrows move the knight. W is up on the screen. Diagonal speed is the same as straight speed.
- **Max speed and acceleration.** Select `Assets/_Project/Data/Character/CharacterMoveConfig.asset`. Change `Max Speed` during Play mode to see the effect at once. Turn off `Instant Acceleration` to get a smooth ramp controlled by `Acceleration`.
- **Collisions.** Walk into walls, pillars, barrels, crates and the table. The knight stops and slides along walls without sticking. The collider covers only the feet, so the knight can stand right against a wall face.
- **Y-sorting.** Walk around a pillar or a barrel. From below the knight is drawn in front of it, from above behind it.
- **Animation and facing.** Idle and walk animations in four directions, left is the mirrored side sprite. When moving diagonally the knight keeps the direction it was already facing. After stopping it keeps facing the last direction.
- **Camera.** Cinemachine camera follows the knight with light damping, no jitter while moving.

### Task 3, enemy AI
TODO

### Task 4, combat
TODO

## Project structure
TODO

## Decisions
- View: 3/4 top-down instead of a strict isometric grid. I asked the team, both were accepted. It matches Nanokings and keeps all art from one author.
- W moves the player up the screen, S down, A left, D right.
- Acceleration is instant by default. A smooth ramp can be enabled in `CharacterMoveConfig`, where max speed is also set.
- Camera: Cinemachine follow camera, no custom script.
- Animation: states are switched from code with `Animator.Play`, the controller has no transitions. Enemies will reuse the same states with their own clips through an Animator Override Controller.

More details in [docs/PLAN.md](docs/PLAN.md#decisions).

## Assets
- [Voxel Dungeon Pack](https://colefoster.itch.io/voxel-dungeon-pack) and [Voxel Crypt Pack](https://colefoster.itch.io/voxel-crypt-pack) by Cole Foster, CC0 1.0. Characters, props, effects and tiles. License files are in `Assets/_Project/Art/ThirdParty/ColeFoster/`. According to the pack README, the sprites are rendered by a generator whose code was written with AI coding agents; no image-generation model was used.

## AI usage
I discussed architecture with AI and used it for research, code review and git housekeeping. All gameplay code was written by me. Editor tooling and the test level layout were made with AI assistance.
