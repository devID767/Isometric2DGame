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
- **Max speed and acceleration.** Select `Assets/_Project/Data/Character/PlayerMoveConfig.asset`. Change `Max Speed` during Play mode to see the effect at once. Turn off `Instant Acceleration` to get a smooth ramp controlled by `Acceleration`.
- **Collisions.** Walk into walls, pillars, barrels, crates and the table. The knight stops and slides along walls without sticking. The collider covers only the feet, so the knight can stand right against a wall face.
- **Y-sorting.** Walk around a pillar or a barrel. From below the knight is drawn in front of it, from above behind it.
- **Animation and facing.** Idle and walk animations in four directions, left is the mirrored side sprite. When moving diagonally the knight keeps the direction it was already facing. After stopping it keeps facing the last direction.
- **Camera.** Cinemachine camera follows the knight with light damping, no jitter while moving.

### Task 3, enemy AI
There are two skeletons. `Skeleton` walks between fixed waypoints to the right of the knight's start position. `Skeleton (Area)` wanders in the room with pillars, above the start. Select a skeleton to see its ranges as gizmos: yellow is detect, grey is lose, red is attack. Select `SkeletonPatrol` or `SkeletonArea` to see its route.
- **Idle and patrol.** A skeleton walks to a point, waits a moment, then picks the next one. The waypoint skeleton goes around its points in order. The area skeleton picks a random point inside its circle each time, never inside a wall or a pillar and never behind one, so it does not get stuck.
- **Chase.** Walk into the yellow circle. The skeleton follows the knight. It sees only in a straight line: hide behind a wall or a pillar and it does not notice you.
- **Search.** Run out of the grey circle or break line of sight. The skeleton walks to the place where it last saw you, looks left and right for a few seconds, then goes back to patrolling. The waypoint skeleton returns to its nearest point. Step back into view during the search and the chase starts again.
- **Attack.** Let it come close. The skeleton stops, turns to the knight, plays the attack animation and logs `Attacking with damage ...` in the Console on a cooldown. It stays in attack a bit past the attack range, so it does not jitter between chase and attack. The lose radius is bigger than the detect radius for the same reason.
- **Tuning.** Radii, attack range, damage, cooldown, idle and search times are in `Assets/_Project/Data/Enemy/EnemyConfig.asset`. Speed is in `Assets/_Project/Data/Character/EnemyMoveConfig.asset`. Speed, chase speed and attack range can be changed during Play mode. Other enemy values are read when the enemy starts, so restart Play mode after changing them. The patrol area radius is on the `SkeletonArea` object.

### Task 4, combat
TODO

## Project structure
TODO

## Decisions
- View: 3/4 top-down instead of a strict isometric grid. I asked the team, both were accepted. It matches Nanokings and keeps all art from one author.
- W moves the player up the screen, S down, A left, D right.
- Acceleration is instant by default. A smooth ramp can be enabled in `CharacterMoveConfig`, where max speed is also set.
- Camera: Cinemachine follow camera, no custom script.
- Enemy AI: a small state machine with a transition table. States (Idle, Patrol, Chase, Search, Attack) do not know about each other, all transitions are declared in one place in `Enemy`.
- Patrol routes are separate components (`WaypointPatrolRoute`, `AreaPatrolRoute`). The patrol state only walks to the next point, the route decides where that point is. A new route type needs no changes in the enemy.
- Player detection: `Physics2D.OverlapCircle` on the Player layer plus a `Linecast` on the Obstacles layer for line of sight. Results are cached once per frame.
- Animation: states are switched from code with `Animator.Play`, the controller has no transitions. The skeleton reuses the knight's controller with its own clips through an Animator Override Controller. Enemy AI states tell the animator what to play and where to face, because attack and look-around cannot be derived from velocity.
- Player and Enemy layers do not collide, so a chasing skeleton cannot push the knight. The skeleton stops at attack range instead.

More details in [docs/PLAN.md](docs/PLAN.md#decisions).

## Assets
- [Voxel Dungeon Pack](https://colefoster.itch.io/voxel-dungeon-pack) and [Voxel Crypt Pack](https://colefoster.itch.io/voxel-crypt-pack) by Cole Foster, CC0 1.0. Characters, props, effects and tiles. License files are in `Assets/_Project/Art/ThirdParty/ColeFoster/`. According to the pack README, the sprites are rendered by a generator whose code was written with AI coding agents; no image-generation model was used.

## AI usage
I discussed architecture with AI and used it for research, code review and git housekeeping. All gameplay code was written by me. Editor tooling, sprite import settings, animation clips, prefabs and the test level layout were set up with AI assistance.
