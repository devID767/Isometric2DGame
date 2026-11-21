using Isometric2DGame.Animations;
using Isometric2DGame.EnemyCharacter.Patrol;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.Movement;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter.States
{
    public class PatrolEnemyState : ITickableState
    {
        private const float Threshold = 0.1f;

        private readonly IPositionHolder _self;
        private readonly CharacterMotor _motor;
        private readonly PatrolRoute _route;
        private readonly CharacterAnimator _animator;
        private readonly EnemyConfig.PatrolSettings _settings;

        private readonly CooldownTimer _patrolPointTimer;

        public bool IsArrived { get; private set; }

        private Vector2 _point;

        public PatrolEnemyState(IPositionHolder self, CharacterMotor motor, PatrolRoute route, CharacterAnimator animator, EnemyConfig.PatrolSettings settings)
        {
            _self = self;
            _motor = motor;
            _animator = animator;
            _route = route;
            _settings = settings;

            _patrolPointTimer = new CooldownTimer(_settings.PointTimeout);
        }

        public void Enter()
        {
            IsArrived = !_route.TryGetNextPoint(_self.Position, out _point);
            _patrolPointTimer.Reset();
        }

        public void Tick(float deltaTime)
        {
            if (IsArrived)
                return;

            _patrolPointTimer.Tick(deltaTime);

            if (_patrolPointTimer.IsDone)
            {
                IsArrived = true;
                return;
            }

            if ((_point - _self.Position).sqrMagnitude < Threshold * Threshold)
            {
                IsArrived = true;
                return;
            }

            _motor.SetDirection((_point - _self.Position).normalized);
            _animator.PlayLocomotion(_motor.Velocity);
        }

        public void Exit()
        {
            _motor.Reset();
            IsArrived = false;
        }
    }
}