using Isometric2DGame.Animations;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.Movement;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    public class ChaseEnemyState : ITickableState
    {
        private readonly CharacterMotor _motor;
        private readonly PlayerSensor _sensor;
        private readonly IPositionHolder _positionHolder;
        private readonly CharacterAnimator _animator;
        private readonly EnemyConfig.ChaseSettings _settings;

        public ChaseEnemyState(IPositionHolder positionHolder, CharacterMotor motor, PlayerSensor sensor, CharacterAnimator animator, EnemyConfig.ChaseSettings settings)
        {
            _positionHolder = positionHolder;
            _motor = motor;
            _sensor = sensor;
            _animator = animator;
            _settings = settings;
        }

        public void Enter()
        {
        }

        public void Tick(float deltaTime)
        {
            _motor.SetDirection(GetDirection(), _settings.SpeedMultiplier);
            _animator.PlayLocomotion(_motor.Velocity);
        }

        private Vector2 GetDirection()
        {
            return (_sensor.Target.Position - _positionHolder.Position).normalized;
        }

        public void Exit()
        {
            _motor.Reset();
        }
    }
}