using Isometric2DGame.Animations;
using Isometric2DGame.Combat;
using Isometric2DGame.Combat.Healths;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.Movement.Modifiers;

namespace Isometric2DGame.MainCharacter
{
    public class PlayerHitReaction
    {
        private readonly CooldownTimer _timer;
        private readonly CooldownTimer _timerInvulnerable;

        private readonly IPositionHolder _self;
        private readonly PlayerCombatConfig.HurtSettings _settings;
        private readonly CharacterAnimator _animator;
        private readonly IReadOnlyHealth _health;
        private readonly Knockback _knockback;

        public bool IsActive => !_timer.IsDone;
        public bool IsInvulnerable => !_timerInvulnerable.IsDone;

        public PlayerHitReaction(IPositionHolder self, PlayerCombatConfig.HurtSettings settings, Knockback knockback, CharacterAnimator animator, IReadOnlyHealth health)
        {
            _self = self;
            _settings = settings;
            _knockback = knockback;
            _animator = animator;
            _health = health;

            _timer = new CooldownTimer(_settings.Duration);
            _timerInvulnerable = new CooldownTimer(_settings.InvulnerableDuration);
        }

        public void Enable()
        {
            _timerInvulnerable.SetDone();
            _timer.SetDone();
            _knockback.Reset();

            _health.Damaged += OnDamaged;
        }

        private void OnDamaged(DamageInfo info)
        {
            if (!_health.Data.IsAlive)
                return;
            
            _knockback.Apply(info.DirectionTo(_self.Position) * _settings.PushSpeed);
            
            _timerInvulnerable.Reset();
            _timer.Reset();
            _animator.PlayAnimation(CharacterAction.Hurt, true);
        }

        public void Tick(float deltaTime)
        {
            _timer.Tick(deltaTime);
            _timerInvulnerable.Tick(deltaTime);
        }

        public void Disable()
        {
            _health.Damaged -= OnDamaged;
        }
    }
}
