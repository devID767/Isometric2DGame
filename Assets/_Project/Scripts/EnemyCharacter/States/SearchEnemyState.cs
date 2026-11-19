using Isometric2DGame.Animations;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.Movement;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    public class SearchEnemyState : ITickableState
    {
        private const float DistanceThreshold = 0.1f;

        private readonly PlayerSensor _sensor;
        private readonly IPositionHolder _self;
        private readonly CharacterAnimator _animator;
        private readonly CharacterMotor _motor;
        private readonly EnemyConfig.SearchSettings _settings;

        private readonly Timer _holdTimer;
        private readonly Timer _extraTimer;
        private readonly Timer _holdAnimationTimer;

        private bool _lastRotationIsRight;

        public bool IsDone => _holdTimer.IsDone || _extraTimer.IsDone;

        private Vector2 _targetPosition;

        public SearchEnemyState(IPositionHolder self, PlayerSensor sensor, CharacterAnimator animator, CharacterMotor motor, EnemyConfig.SearchSettings settings)
        {
            _self = self;
            _sensor = sensor;
            _animator = animator;
            _motor = motor;
            _settings = settings;

            _holdTimer = new Timer(_settings.HoldTime);
            _holdAnimationTimer = new Timer(_settings.HoldTime / _settings.LookCount);
            _extraTimer = new Timer(_settings.Timeout);
        }

        public void Enter()
        {
            _targetPosition = _sensor.LastSeenPosition;
            _holdTimer.Reset();
            _extraTimer.Reset();
            _holdAnimationTimer.Reset();
        }

        public void Tick(float deltaTime)
        {
            _extraTimer.Tick(deltaTime);

            if ((_targetPosition - _self.Position).sqrMagnitude > DistanceThreshold * DistanceThreshold)
            {
                var direction = (_targetPosition - _self.Position).normalized;
                _motor.SetDirection(direction);
                _animator.PlayLocomotion(_motor.Velocity);
            }
            else
            {
                _holdAnimationTimer.Tick(deltaTime);
                if (_holdAnimationTimer.IsDone)
                {
                    var rotation = _lastRotationIsRight ? Vector2.left : Vector2.right;
                    _lastRotationIsRight = !_lastRotationIsRight;
                    _animator.Face(rotation);
                    _holdAnimationTimer.Reset();
                }
                
                _motor.Reset();
                _animator.PlayAnimation(CharacterAction.Idle);
                _holdTimer.Tick(deltaTime);
            }
        }

        public void Exit()
        {
            _motor.Reset();
        }
    }
}