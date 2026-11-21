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
- Attack: Space, Enter, left mouse button or gamepad west button (X / Square)
- Fireball: casts on its own, no button

## How to test
Open `Assets/_Project/Scenes/Game.unity` and press Play.

### Task 2, movement and camera
- **Movement.** WASD or arrows move the knight. W is up on the screen. Diagonal speed is the same as straight speed.
- **Max speed and acceleration.** Select `Assets/_Project/Data/Player/PlayerMoveConfig.asset`. Change `Max Speed` during Play mode to see the effect at once. Turn off `Instant Acceleration` to get a smooth ramp controlled by `Acceleration`.
- **Collisions.** Walk into walls, pillars, barrels, crates and the table. The knight stops and slides along walls without sticking. The collider covers only the feet, so the knight can stand right against a wall face.
- **Y-sorting.** Walk around a pillar or a barrel. From below the knight is drawn in front of it, from above behind it.
- **Animation and facing.** Idle and walk animations in four directions, left is the mirrored side sprite. When moving diagonally the knight keeps the direction it was already facing. After stopping it keeps facing the last direction.
- **Camera.** Cinemachine camera follows the knight with light damping, no jitter while moving.

### Task 3, enemy AI
There are two skeletons. `Skeleton` walks between fixed waypoints to the right of the knight's start position. `Skeleton (Area)` wanders in the room with pillars, above the start. Select a skeleton to see its ranges as gizmos: yellow is detect, grey is lose, red is attack. Select `SkeletonPatrol` or `SkeletonArea` to see its route.
- **Idle and patrol.** A skeleton walks to a point, waits a moment, then picks the next one. The waypoint skeleton goes around its points in order. The area skeleton picks a random point inside its circle each time, never inside a wall or a pillar and never behind one, so it does not get stuck.
- **Chase.** Walk into the yellow circle. The skeleton follows the knight. It sees only in a straight line: hide behind a wall or a pillar and it does not notice you.
- **Search.** Run out of the grey circle or break line of sight. The skeleton walks to the place where it last saw you, looks left and right for a few seconds, then goes back to patrolling. The waypoint skeleton returns to its nearest point. Step back into view during the search and the chase starts again.
- **Attack.** Let it come close. The skeleton stops, turns to the knight, plays the attack animation and hits the knight on a cooldown (see Task 4). It stays in attack a bit past the attack range, so it does not jitter between chase and attack. The lose radius is bigger than the detect radius for the same reason.
- **Tuning.** Radii, attack range, damage, cooldown, idle and search times are in `Assets/_Project/Data/Enemy/EnemyConfig.asset`. Speed is in `Assets/_Project/Data/Enemy/EnemyMoveConfig.asset`. Speed, chase speed and attack range can be changed during Play mode. Other enemy values are read when the enemy starts, so restart Play mode after changing them. The patrol area radius is on the `SkeletonArea` object.

### Task 4, combat
Select the knight to see its ranges as gizmos: red is the sword hit circle in front of it (Play mode only), blue is the fireball range.
- **Melee attack.** Press Space or click. The knight swings the sword in the direction it faces and slows down during the swing. Damage happens in the middle of the swing, not on the key press. One swing hits every skeleton in the circle once, even if a skeleton has several colliders. A skeleton has 3 HP, the sword deals 1.
- **Fireball.** Every 1.5 seconds the knight casts a fireball at the nearest skeleton in range that it can see. Stand behind a pillar or a wall and it stops casting. A fireball disappears when it hits a skeleton or a wall, or after 1 second. Fireballs come from a pool, nothing is created after the first few casts.
- **Enemy hit reaction.** A hit skeleton flashes red, is pushed back, plays the hurt animation and cancels its own attack. After that it goes back to chasing if it still sees the knight. At 0 HP it plays the death animation, its health bar disappears and it stops blocking the way.
- **Enemy attack.** A skeleton next to the knight swings on a cooldown and deals 1 damage. Damage lands in the middle of the swing, so you can step away from a swing that has started.
- **Player hit reaction.** The knight flashes, is pushed back, plays the hurt animation and the camera shakes a little. For a short time after a hit the knight cannot take damage again, so two skeletons cannot drain all HP at once. A hit cancels the knight's own swing.
- **Health UI.** The knight's health bar is in the top left corner, each skeleton has a small bar above its head. Both update on every hit.
- **Death and restart.** The knight has 5 HP. At 0 the knight plays the death animation, input stops, and a second later the Game Over screen shows up. Restart reloads the scene with full health and all skeletons back in place. The button works with the mouse, Enter or a gamepad.
- **Tuning.** Player HP, sword, fireball and hit reaction values are in `Assets/_Project/Data/Player/PlayerCombatConfig.asset`. Skeleton HP, damage, attack timing and hit reaction are in `Assets/_Project/Data/Enemy/EnemyConfig.asset`. Most values are read when the object starts, so restart Play mode after changing them.

## Project structure
```
Assets/_Project/
  Art/          sprites, animation clips and controllers, third party art with licenses
  Data/         ScriptableObject configs for the player and the enemy
  Editor/       sprite sheet slicer
  Input/        input actions and the generated C# wrapper
  PhysicsMaterials/ zero friction material for characters
  Prefabs/      Skeleton, Fireball
  Scenes/       Game.unity, the only scene
  Scripts/
    Core/           GameController: player death, Game Over, restart
    MainCharacter/  Player and its parts: input, melee attack, hit reaction
    EnemyCharacter/ Enemy, its states, patrol routes and the player sensor
    StateMachines/  generic state machine with a transition table
    Combat/         health, damage, target query, projectiles
    Movement/       CharacterMotor and velocity modifiers (speed scale, knockback)
    Animations/     CharacterAnimator: picks clips by action and facing
    Effects/        hit flash and camera shake
    Ui/             health bars and the Game Over screen
    Infrastructure/ small shared helpers: timer, component pool
```
Namespaces follow the folders, for example `Isometric2DGame.EnemyCharacter.States`. `Player` and `Enemy` are thin MonoBehaviours. They create plain C# classes in `Awake` and forward Unity callbacks to them, so most of the logic does not depend on the scene.

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
- Damage has one path. Everything that hits calls `IDamageable.TakeDamage`, `Health` raises `Damaged`, `Changed` and `Died`. Hit flash, camera shake, health bars and hurt reactions subscribe to these events and know nothing about who dealt the damage.
- Melee and enemy attacks check the hit area with a physics query at the moment of the hit, not with a collider on the weapon. Swing timing (hit delay, duration, cooldown) is one `Swing` class shared by the player and the skeleton.
- Fireball is an automatic weapon: it picks the nearest visible enemy, flies with a `CircleCast` each physics step instead of a trigger, so it cannot pass through a target at any speed, and is reused from an object pool.
- Speed changes and knockback are velocity modifiers on `CharacterMotor`, so a slow attack, a faster chase and a push stack without knowing about each other.
- `GameController` connects the player to the HUD and the Game Over screen. The player and the UI do not reference each other. The game has no pause, so `Time.timeScale` is never changed.

More details in [docs/PLAN.md](docs/PLAN.md#decisions).

## Assets
- [Voxel Dungeon Pack](https://colefoster.itch.io/voxel-dungeon-pack) and [Voxel Crypt Pack](https://colefoster.itch.io/voxel-crypt-pack) by Cole Foster, CC0 1.0. Characters, props, effects and tiles. License files are in `Assets/_Project/Art/ThirdParty/ColeFoster/`. According to the pack README, the sprites are rendered by a generator whose code was written with AI coding agents; no image-generation model was used.
- [Hearthmark: Pixel RPG UI](https://colefoster.itch.io/hearthmark-rpg-ui) by Cole Foster, CC0 1.0. UI panels, buttons, health bar and the pixel font. Only the used files are included, the license is in `Assets/_Project/Art/ThirdParty/ColeFoster/Hearthmark/`.

## AI usage
I discussed architecture with AI and used it for research, code review and git housekeeping. All gameplay code was written by me. Editor tooling, sprite import settings, animation clips, prefabs, the UI layout in the scene and the test level layout were set up with AI assistance.
