using Isometric2DGame.Animations;
using Isometric2DGame.Combat;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.Movement;
using Isometric2DGame.StateMachines;

namespace Isometric2DGame.EnemyCharacter
{
    public class HurtEnemyState : ITickableState
    {
        private readonly EnemyConfig.HurtSettings _settings;
        private readonly IPositionHolder _self;
        private readonly CharacterAnimator _animator;
        private readonly IReadOnlyHealth _health;
        private readonly Knockback _knockback;

        private readonly CooldownTimer _durationTimer;

        public bool IsTriggered { get; private set; }
        public bool IsDone => _durationTimer.IsDone;

        public HurtEnemyState(EnemyConfig.HurtSettings settings, IPositionHolder self, CharacterAnimator animator, IReadOnlyHealth health, Knockback knockback)
        {
            _settings = settings;
            _self = self;
            _animator = animator;
            _health = health;
            _knockback = knockback;

            _durationTimer = new CooldownTimer(_settings.Duration);

            _health.Damaged += OnDamaged;
        }

        public void Enter()
        {
            IsTriggered = false;
            _animator.PlayAnimation(CharacterAction.Hurt, restart: true);
            _durationTimer.Reset();
        }

        public void Tick(float deltaTime)
        {
            _durationTimer.Tick(deltaTime);
        }

        public void Exit()
        {
        }

        private void OnDamaged(DamageInfo info)
        {
            if (!_health.Data.IsAlive)
                return;
                
            _knockback.Apply(info.DirectionTo(_self.Position) * _settings.PushSpeed);
            IsTriggered = true;
        }
    }
}