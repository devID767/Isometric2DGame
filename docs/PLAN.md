# Plan

![Workflow](workflow.png)

Core tasks first, stretch goals only after all four are merged into `main`. Estimates are focused hours, breaks not included.

## Estimates

| Block | Estimate | Actual |
|---|---|---|
| Planning | 1.5 h | ~2 h |
| Asset sourcing | 2.5 h | ~1.5 h |
| Task 1 - Project setup | 2 h | ~1 h |
| Task 2 - Movement and camera | 4 h | ~3.5 h |
| Task 3 - Enemy AI | 5 h | ~4.5 h |
| Task 4 - Combat | 5 h | ~9 h |
| Polish and QA | 4 h | ~6.5 h |
| Submission | 1 h | ~1 h |
| **Total** | **25 h** | **~29 h** |

Actual time comes from commit times and my work sessions, breaks removed.

- **Combat: 9 h instead of 5.** It had the longest list of the four tasks but the same estimate as the enemy AI. Hurt and death had to work for both the enemy and the player, the fireball needed a pool, invulnerability frames came up in testing.
- **Polish: 6.5 h instead of 4.** About 3 h were done inside the combat branch (death clips, pixel UI, cleanup). The rest went into stretch goals (spawner, new level, lighting) and balance after a playtest.
- Setup, movement and asset sourcing were faster than planned and covered part of it.

## Scope

| Block | Content |
|---|---|
| Task 1 | Unity 6 URP 2D project, git, public repo, README |
| Task 2 | Input System, Rigidbody2D movement, Y-sorted tilemap, Cinemachine, four-direction animation |
| Task 3 | State machine (idle, patrol, chase, search, attack), sensor with line of sight, waypoint and random area routes |
| Task 4 | Sword, auto fireball, health, health UI, hit flash, knockback, camera shake, death, Game Over, restart |
| Polish | Level with three zones, enemy spawner with a pool, hurt and death animations, pixel UI, playtest and balance |
| Stretch | Done: lighting and post-processing, attack and death animations. Dropped: hit particles, alert marker. NavMesh: not done |

## Decisions

**Setup and art**
- **3/4 top-down view, not a strict isometric grid** (17.11). The team said either is fine. It matches Nanokings and lets all art come from one author.
- **Cole Foster CC0 packs, no Asset Store art.** One consistent source, and the Asset Store EULA does not allow the files in a public repository.

**Movement**
- **W is up on the screen.** In a 3/4 view the grid matches the screen axes, input goes to the motor as is.
- **Instant acceleration by default**, a smooth ramp is one toggle in `CharacterMoveConfig`. Both readings of the requirement are covered.
- **Logic in plain C# classes.** `Player` and `Enemy` create `CharacterMotor`, `CharacterAnimator` and the rest in `Awake` and forward Unity callbacks (Humble Object). State is reset in `OnEnable`.
- **Animation switched from code** with `Animator.Play` by action and facing, no transitions in the controller. Attack, hurt and death need exact timing. The skeleton reuses the controller through an Animator Override Controller.

**Enemy AI**
- **State machine with a transition table** (18.11). States implement `Enter`/`Tick`/`Exit` and never switch themselves. All `(from, to, condition)` transitions are declared in one method, any-transitions cover hurt and death.
- **Detection by query.** `OverlapCircle` on the Player layer plus a `Linecast` on Obstacles, cached once per frame.
- **Hysteresis.** Lose radius > detect radius, attack exit range > attack range, otherwise the enemy flickers on a border.
- **Patrol route as a strategy.** `WaypointPatrolRoute` and `AreaPatrolRoute` share one abstract `PatrolRoute`, the patrol state only walks to the next point. The area route checks that a point is free and reachable in a straight line.
- **"Dynamic patrol" read as two things:** random area routes, and a search state that walks to the last seen position and looks around.
- **State shown by animation** instead of colour: walk, idle, attack and look-around already differ.
- **Chase is faster than patrol but slower than the player** (2.8 vs 3), so the player can escape.

**Combat**
- **One damage path** (19.11). Every hit calls `IDamageable.TakeDamage`, `Health` raises `Damaged`, `Changed`, `Died`. Flash, shake, health bars and hurt reactions subscribe. Outside code only sees `IReadOnlyHealth`.
- **Hits by query at the hit moment**, no weapon colliders. `TargetQuery` collects unique targets, so a body with several colliders takes damage once.
- **Shared `Swing` timing** for the knight and the skeleton. Damage lands mid-swing, so a started swing can be dodged.
- **Clip length follows gameplay timing.** `CharacterAnimator.SetDuration` scales animator speed to the config value.
- **Velocity modifiers** on `CharacterMotor`: slow attack, faster chase and knockback stack, each owner changes only its own modifier.
- **No state machine for the player.** Free, swinging and hurt are two flags in `Player.Update`.
- **Invulnerability after a hit**, so two skeletons cannot drain all HP at once.
- **Fireball as the automatic weapon** (21.11). It picks the nearest visible enemy and moves by `CircleCast`, because triggers are not part of continuous collision and a fast one can pass through a target.
- **`GameController` as a mediator.** It binds HUD, spawner and Game Over, restart reloads the scene. No pause, `Time.timeScale` is never changed.
- **Legacy UI `Text` with the Hearthmark pixel font.** No TextMesh Pro package for two labels.

**Polish**
- **Small level with three zones** (21.11): a safe start room, a pillar hall where line of sight matters, an open arena. The brief is about mechanics, corridors would only add walking.
- **One enemy pool for all spawn points.** The spawn point gives the skeleton its route before it is turned on, `PatrolEnemyState` reads it through `IPatrolRouteHolder` when patrol starts.
- **`ComponentPool.Get(setup)` runs the setup before activation**, so `OnEnable` already sees the new position and route.
- **`Enemy` does not know about the pool.** The dead state raises `CorpseExpired`, the spawner subscribes on spawn and unsubscribes before release.
- **Random spawn point out of reach of the player.** The nearest one made all skeletons come from the same place.
- **Unity lifecycle, no custom entry point.** No object reaches into another during initialization, so their order does not matter.
- **Balance against kiting.** Running while the fireball killed everything was the best tactic. The fireball got weaker and the chase faster.
