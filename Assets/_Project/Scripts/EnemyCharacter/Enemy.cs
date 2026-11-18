using Isometric2DGame.Animations;
using Isometric2DGame.Environment;
using Isometric2DGame.Movement;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour, IPositionHolder
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

        public Vector2 Position => _rb.position;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sensor = new PlayerSensor(this, _enemyConfig);
            _motor = new CharacterMotor(_rb, _moveConfig);
            _characterAnimator = new CharacterAnimator(_animator, _spriteRenderer);

            _stateMachine = CreateStateMachine();
        }

        private void OnEnable()
        {
            _characterAnimator.Reset();
            _stateMachine.Restart();
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
        }

        private void OnDrawGizmosSelected()
        {
            if (_enemyConfig == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _enemyConfig.DetectRadius);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, _enemyConfig.LoseRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _enemyConfig.AttackRange);
        }

        private StateMachine CreateStateMachine()
        {
            var idle = new IdleEnemyState(_enemyConfig.IdleTime, _characterAnimator);
            var patrol = new PatrolEnemyState(this, _motor, _patrolRoute, _characterAnimator);
            var chase = new ChaseEnemyState(this, _motor, _sensor, _characterAnimator);
            var search = new SearchEnemyState(this, _sensor, _characterAnimator, _motor, _enemyConfig.SearchHoldTime);
            var attack = new AttackEnemyState(this, _sensor, _enemyConfig.AttackDamage, _enemyConfig.AttackCooldown, _characterAnimator);
            
            var stateMachine = new StateMachine(idle);

            var detect = _enemyConfig.DetectRadius;
            var lose = _enemyConfig.LoseRadius;

            stateMachine.AddTransition(idle, patrol, () => idle.IsDone);
            stateMachine.AddTransition(patrol, idle, () => patrol.IsArrived);
            stateMachine.AddTransition(idle, chase, () => CanSee(detect));
            stateMachine.AddTransition(patrol, chase, () => CanSee(detect));
            stateMachine.AddTransition(chase, attack, () => _sensor.Distance < _enemyConfig.AttackRange);
            stateMachine.AddTransition(attack, chase, () => _sensor.Distance > _enemyConfig.AttackRange + _enemyConfig.AttackExitMargin);
            stateMachine.AddTransition(chase, search, () => !CanSee(lose));
            stateMachine.AddTransition(search, chase, () => CanSee(detect));
            stateMachine.AddTransition(search, idle, () => search.IsDone);

            bool CanSee(float radius) => _sensor.CanSee(radius);

            return stateMachine;
        }
    }
}