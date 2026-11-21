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
- Random patrol area and search state
- Health component shared with the player (moved to Task 4, where damage is needed)
- State shown by animation instead of colour, debug gizmos in the editor

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
- Alert marker over the enemy: "!" on chase, "?" on search
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
4. NavMesh pathfinding for enemies - 2 h. Chase moves in a straight line and catches on corners

## Decisions

Added as they are made.

- **View: 3/4 top-down instead of a strict isometric grid** (17.11). I asked the team which one they expect, the answer was that either is fine. I chose 3/4 because Nanokings uses it and because all art can then come from one author with the same pixel density and palette.
- **W moves the player up the screen.** With a 3/4 view the grid matches the screen axes, so input goes to the motor without conversion.
- **Art: Cole Foster packs (CC0)** for characters, enemies, props and tiles. No free kit matched Nanokings closely, so I picked one consistent CC0 source over mixing authors. The sprites are rendered by a generator whose code was written with AI coding agents. This is stated in the credits.
- **No Unity Asset Store art.** The standard Asset Store EULA does not allow putting the files in a public repository, and reviewers need to open the project as is.
- **Instant acceleration by default.** `CharacterMoveConfig` has max speed, an instant acceleration toggle and an acceleration value for a smooth ramp, so both readings of the requirement are covered.
- **Movement logic in plain C# classes.** `CharacterMotor` and `PlayerInputReader` are not MonoBehaviours. `Player` creates them and forwards Unity callbacks (Humble Object). Enemies will reuse the same motor.
- **Camera: Cinemachine 3 follow camera, no custom script.** Rigidbody interpolation is on to avoid jitter.
- **Animation states are switched from code, not by Animator transitions.** `CharacterAnimator` picks the state from action and facing (`Walk_Side`, `Idle_Up`, ...) and calls `Animator.Play` only when it changes. Attack, hurt and death in Task 4 need exact timing, which is easier to follow in code than in a transition graph. Enemies will use the same state names with their own clips through an Animator Override Controller.
- **Enemy state machine with a transition table** (18.11). States implement `Enter`/`Exit` and optionally `Tick`/`FixedTick`. They do not switch states themselves: `StateMachine` stores `(from, to, condition)` transitions and checks them before ticking the current state, first match wins. All enemy behaviour is readable in one method, and a state never changes the state while it is still running its own `Enter` or `Exit`.
- **Player detection by query, not by trigger collider.** `PlayerSensor` runs `OverlapCircle` with the Player mask at the lose radius and a `Linecast` with the Obstacles mask, once per frame in `Refresh()`. States and transitions read the cached values. Without a target the distance is infinity, so range checks fail without extra null checks.
- **Hysteresis for transitions.** Lose radius is bigger than detect radius, and attack ends at an exit range bigger than the attack range. Without it the enemy flickers between states when the player stands on a border.
- **"Dynamic patrol routes": random area route plus search state.** I read "change or randomize" as two things. Randomize: `AreaPatrolRoute` picks a random point in a circle and checks it with `OverlapCircle` (not inside an obstacle) and `CircleCast` (the body can walk there in a straight line). Change: after losing the player the enemy goes to the last seen position and looks around before returning to its route. Fixed waypoints stay available as `WaypointPatrolRoute`, which returns to the nearest point after a chase.
- **Patrol route as a strategy.** `PatrolRoute` is an abstract MonoBehaviour with `TryGetNextPoint`. The patrol state walks to the point and reports arrival, the route decides which point. A MonoBehaviour and not an interface, so the route can be assigned in the Inspector.
- **States drive the animation.** `CharacterAnimator` no longer decides the action from velocity on its own. It has `Face`, `PlayAnimation` and `PlayLocomotion`, and each state calls what it needs. Attack and look-around need an explicit action and facing that velocity cannot give.
- **Enemy reuses the player's motor and animator.** Same `CharacterMotor` with its own `EnemyMoveConfig`, same Animator Controller through an Animator Override Controller with skeleton clips.
- **Health moved to Task 4, enemy state shown by animation** (19.11). Health only matters once something deals damage, so it is built together with combat. The brief allows colour or gizmos for the enemy state. Patrol, chase and attack already look different through walk, idle and attack clips, and search adds the look-around. Chase and patrol share the walk clip, which is why an alert marker is planned for polish.
- **`EnemyConfig` grouped into nested settings** (19.11). Detection and each state (idle, patrol, chase, attack, search) get a `[Serializable]` class inside the one config asset, and each state receives only its group. One enemy type does not justify several config assets, and the groups keep the inspector readable and the constructors short. Patrol and search timeouts and the number of look-around turns moved there from constants.
- **Faster chase.** The chase state passes a speed multiplier (1.25) to `CharacterMotor.SetDirection`, so the enemy is faster than on patrol but still slower than the player (2.5 vs 3), and the player can escape. The multiplier goes with the direction, so no state has to reset it.
- **Attack needs line of sight.** Chase switches to attack only with `CanSee(attack range)`, and attack returns to chase when the line of sight is lost, so the enemy cannot hit through a wall.
- **One damage path through `Health` events** (19.11). Every attack calls `IDamageable.TakeDamage(DamageInfo)`. `Health` changes the value and raises `Damaged`, `Changed` and `Died`. Reactions are subscribers: the hurt state and the player hit reaction push the character back, `DamageFlash` blinks the sprite, `DamageInpulse` shakes the camera, `HealthBarUi` redraws. A new effect does not touch any attack code. Outside code sees `IReadOnlyHealth` with a `HealthData` snapshot, so only the owner can change health.
- **Hit check by query at the hit moment.** The sword and the skeleton do not have weapon colliders. At the hit frame the player runs `OverlapCircle` in front of it, the skeleton checks that the player is still within the exit range and visible. Each swing hits a target once. `TargetQuery` collects unique targets by their rigidbody, so a body with several colliders takes damage once.
- **Shared `Swing` timing.** Hit delay, duration and cooldown live in one class used by both the player attack and the enemy attack state. Damage lands in the middle of the animation, so a started swing can be dodged and an attack reads as an attack.
- **Animation speed from the clip length.** `CharacterAnimator.SetDuration` scales the animator speed so that a clip lasts exactly as long as the gameplay timer (swing, hurt). Timings are changed in the config, clips do not need to be re-timed.
- **Enemy hurt and death as states with any-transitions.** `StateMachine.AddAnyTransition` leaves any state for hurt or dead. Hurt can re-enter itself, so a second hit restarts the reaction. Dead has the highest priority and has no way out.
- **No state machine for the player.** The player has only three situations: free, swinging, hurt. Two flags (`Swinging`, `IsActive` of the hit reaction) in `Player.Update` are easier to read than a state machine for that. Being hit cancels the swing.
- **Invulnerability after a hit.** The player ignores damage for a short time after each hit, so two skeletons cannot take all HP in one moment.
- **Velocity modifiers.** `CharacterMotor` passes the input velocity through a list of `IVelocityModifier`. `SpeedScale` covers the slow attack and the faster chase, `Knockback` adds a push that fades out. Each owner holds its own modifier and changes only it, nobody has to restore the speed someone else changed.
- **Composition root in `Awake`.** `Player` and `Enemy` create all their parts in `Awake` and pass each part only the config group and the objects it needs. State is reset in `OnEnable`, so an object that is turned off and on starts clean. This is also what object pooling needs.
- **Enemies collide with each other, not with the player** (20.11). Several skeletons chasing the same knight no longer stack into one sprite. Player and Enemy still do not collide, the skeleton stops at attack range.
- **`GameController` as a mediator** (20.11). It binds the player's health to the HUD bar and shows the Game Over screen one second after death. Restart reloads the scene, which resets everything without extra code. The player does not know about UI, the UI does not know about the player. No pause in the game, so `Time.timeScale` is never changed and does not need restoring.
- **UI: Hearthmark pack and legacy UI Text** (20.11). Same author and pixel density as the rest of the art, CC0. TextMesh Pro is not in the project, and the pixel font renders crisp with legacy `Text` at multiples of its base size, so I did not add a package for two labels.
- **Fireball as an automatic weapon** (21.11). The brief lists an auto-attacking weapon and melee plus ranged as bonuses, one fireball covers both. Every interval it picks the nearest enemy in range with a clear line of sight, so it does not waste shots into walls. It is independent from the sword: no button, no animation lock.
- **Projectile moves by `CircleCast`, not by trigger.** Box2D triggers are not part of continuous collision detection, so a fast trigger can pass through a thin target. The projectile casts the distance of the next step and moves only if nothing is in the way. The first hit is deterministic, no separate projectile layer or collision matrix setup is needed, the hit mask comes from the config.
- **Generic `ComponentPool<T>`** over `UnityEngine.Pool.ObjectPool`. Projectiles are taken from it now. Enemy spawning in polish will use the same pool.
