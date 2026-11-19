using Isometric2DGame.Animations;
using Isometric2DGame.Combat;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    public class AttackEnemyState : ITickableState
    {
        private readonly CharacterAnimator _animator;
        private readonly IPositionHolder _self;
        private readonly PlayerSensor _sensor;
        private readonly EnemyConfig.AttackSettings _settings;

        private readonly Swing _swing;

        public bool Swinging => _swing.Swinging;

        public AttackEnemyState(EnemyConfig.AttackSettings settings, IPositionHolder self, PlayerSensor sensor, CharacterAnimator animator)
        {
            _settings = settings;
            _animator = animator;
            _self = self;
            _sensor = sensor;

            _swing = new Swing(settings.Swing);
            _animator.SetDuration(CharacterAction.Attack, _settings.Swing.Duration);
        }

        public void Enter()
        {
            _swing.Reset();
        }

        public void Exit()
        {
        }

        public void Tick(float deltaTime)
        {
            if (_swing.TryStart())
            {
                PlayAnimation();
            }

            if (_swing.Tick(deltaTime))
            {
                TryHit();
            }
        }

        private void PlayAnimation()
        {
            _animator.Face(_sensor.Target.Position - _self.Position);
            _animator.PlayAnimation(CharacterAction.Attack, true);
        }

        private void TryHit()
        {
            if (_sensor.CanSee(_settings.ExitRange))
            {
                _sensor.Target.TakeDamage(_settings.Damage);
                Debug.Log($"Attacking with damage {_settings.Damage}");
            }
            else
            {
                Debug.Log($"Miss attack");
            }
        }
    }
}