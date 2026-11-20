using Isometric2DGame.Animations;
using Isometric2DGame.Combat;
using Isometric2DGame.Environment;
using Isometric2DGame.Movement;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour, ITarget
    {
        [SerializeField] private PatrolRoute _patrolRoute;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterMoveConfig _moveConfig;

        [SerializeField] private EnemyConfig _enemyConfig;

        private Rigidbody2D _rb;
        private PlayerSensor _sensor;
        private CharacterMotor _motor;
        private CharacterAnimator _characterAnimator;
        private StateMachine _stateMachine;
        private Health _health;

        public Vector2 Position => _rb.position;

        public bool IsAlive => _health.IsAlive;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sensor = new PlayerSensor(this, _enemyConfig.Detection);
            _motor = new CharacterMotor(_rb, _moveConfig);
            _characterAnimator = new CharacterAnimator(_animator, _spriteRenderer);
            _health = new Health(_enemyConfig.Health.Max);

            _stateMachine = CreateStateMachine();
        }

        private void OnEnable()
        {
            _rb.simulated = true;

            _characterAnimator.Reset();
            _stateMachine.Restart();
            _health.Reset();

            _health.Died += OnDied;
        }

        public void TakeDamage(int value)
        {
            if (!IsAlive)
                return;

            _health.TakeDamage(value);
        }

        private void OnDied()
        {
            _rb.simulated = false;
        }

        private void Update()
        {
            _sensor.Refresh();
            _stateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _motor.FixedTick();
        }

        private void OnDisable()
        {
            _stateMachine.Stop();
            _motor.Reset();

            _health.Died -= OnDied;
        }

        private void OnDrawGizmosSelected()
        {
            if (_enemyConfig == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _enemyConfig.Detection.DetectRadius);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, _enemyConfig.Detection.LoseRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _enemyConfig.Attack.Range);
            Gizmos.color = Color.purple;
            Gizmos.DrawWireSphere(transform.position, _enemyConfig.Attack.ExitRange);
        }

        private StateMachine CreateStateMachine()
        {
            var idle = new IdleEnemyState(_enemyConfig.Idle.HoldTime, _characterAnimator);
            var patrol = new PatrolEnemyState(this, _motor, _patrolRoute, _characterAnimator, _enemyConfig.Patrol);
            var chase = new ChaseEnemyState(this, _motor, _sensor, _characterAnimator, _enemyConfig.Chase);
            var search = new SearchEnemyState(this, _sensor, _characterAnimator, _motor, _enemyConfig.Search);
            var attack = new AttackEnemyState(_enemyConfig.Attack, this, _sensor, _characterAnimator);
            var hurt = new HurtEnemyState(_enemyConfig.Hurt, _characterAnimator, _health);
            var dead = new DeadEnemyState(_characterAnimator);

            var stateMachine = new StateMachine(idle);

            var detect = _enemyConfig.Detection.DetectRadius;
            var lose = _enemyConfig.Detection.LoseRadius;

            stateMachine.AddAnyTransition(dead, () => !_health.IsAlive);
            stateMachine.AddAnyTransition(hurt, () => hurt.IsTriggered, canTransitionToSelf: true);
            stateMachine.AddTransition(hurt, chase, () => hurt.IsDone && CanSee(lose));
            stateMachine.AddTransition(hurt, idle, () => hurt.IsDone);

            stateMachine.AddTransition(idle, patrol, () => idle.IsDone);
            stateMachine.AddTransition(patrol, idle, () => patrol.IsArrived);
            stateMachine.AddTransition(idle, chase, () => CanSee(detect));
            stateMachine.AddTransition(patrol, chase, () => CanSee(detect));
            stateMachine.AddTransition(chase, attack, () => CanSee(_enemyConfig.Attack.Range));
            stateMachine.AddTransition(attack, chase,
                () => !attack.Swinging && !CanSee(_enemyConfig.Attack.ExitRange));
            stateMachine.AddTransition(chase, search, () => !CanSee(lose));
            stateMachine.AddTransition(search, chase, () => CanSee(detect));
            stateMachine.AddTransition(search, idle, () => search.IsDone);

            bool CanSee(float radius) => _sensor.CanSee(radius);

            return stateMachine;
        }
    }
}