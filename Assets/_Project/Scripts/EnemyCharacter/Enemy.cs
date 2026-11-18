using System.Collections.Generic;
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
        [SerializeField] private Patrols _patrols;
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
            _characterAnimator.Tick(_motor.Velocity);
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
            var idle = new IdleEnemyState(_enemyConfig.IdleTime);
            var patrol = new PatrolEnemyState(this, _motor, _patrols.GetPoints());
            var chase = new ChaseEnemyState(this, _motor, _sensor);
            var attack = new AttackEnemyState();
            
            var stateMachine = new StateMachine(idle);

            stateMachine.AddTransition(idle, patrol, () => idle.IsDone && patrol.HasPoints);
            stateMachine.AddTransition(idle, chase, () => _sensor.CanSee(_enemyConfig.DetectRadius));
            stateMachine.AddTransition(patrol, chase, () => _sensor.CanSee(_enemyConfig.DetectRadius));
            stateMachine.AddTransition(chase, attack, () => _sensor.Distance < _enemyConfig.AttackRange);
            stateMachine.AddTransition(attack, chase, () => _sensor.Distance > _enemyConfig.AttackRange + _enemyConfig.AttackExitMargin);
            stateMachine.AddTransition(chase, idle, () => !_sensor.CanSee(_enemyConfig.LoseRadius));

            return stateMachine;
        }
    }
}