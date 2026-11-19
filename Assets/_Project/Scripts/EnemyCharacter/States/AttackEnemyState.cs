using Isometric2DGame.Animations;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    public class AttackEnemyState : ITickableState
    {
        private readonly Timer _cooldown;
        private readonly Timer _hitDelay;
        private readonly Timer _swingTimer;
        private readonly CharacterAnimator _animator;
        private readonly IPositionHolder _self;
        private readonly PlayerSensor _sensor;
        private readonly EnemyConfig.AttackSettings _settings;

        public bool Swinging { get; private set; }

        private bool _hitDone;

        public AttackEnemyState(EnemyConfig.AttackSettings settings, IPositionHolder self, PlayerSensor sensor, CharacterAnimator animator)
        {
            _settings = settings;
            _animator = animator;
            _self = self;
            _sensor = sensor;

            _cooldown = new Timer(settings.Cooldown);
            _hitDelay = new Timer(settings.HitDelay);
            _swingTimer = new Timer(settings.SwingDuration);
        }

        public void Enter()
        {
            _cooldown.SetDone();
            _hitDelay.Reset();
            Swinging = false;
        }

        public void Exit()
        {
            Swinging = false;
        }

        public void Tick(float deltaTime)
        {
            _cooldown.Tick(deltaTime);

            if (_cooldown.IsDone && !Swinging)
            {
                PlayAnimation();
                _hitDelay.Reset();
                _cooldown.Reset();
                _swingTimer.Reset();
                Swinging = true;
                _hitDone = false;
            }

            if (Swinging)
            {
                _hitDelay.Tick(deltaTime);
                _swingTimer.Tick(deltaTime);

                if (_hitDelay.IsDone && !_hitDone)
                {
                    TryHit();
                    _hitDone = true;
                }

                if (_swingTimer.IsDone)
                    Swinging = false;
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