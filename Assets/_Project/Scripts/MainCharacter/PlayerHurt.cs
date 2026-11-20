using Isometric2DGame.Animations;
using Isometric2DGame.Combat;
using Isometric2DGame.Infrastructure;

namespace Isometric2DGame.MainCharacter
{
    public class PlayerHurt
    {
        private readonly CooldownTimer _timer;
        private readonly Health _health;
        private readonly CharacterAnimator _animator;
        private readonly PlayerMeleeAttack _attack;

        public bool IsActive => !_timer.IsDone;

        public PlayerHurt(float duration, Health health, CharacterAnimator animator, PlayerMeleeAttack attack)
        {
            _timer = new CooldownTimer(duration);

            _health = health;
            _animator = animator;
            _attack = attack;

            _animator.SetDuration(CharacterAction.Hurt, duration);
        }

        public void Enable()
        {
            _timer.SetDone();
            _health.Damaged += OnDamaged;
        }

        private void OnDamaged()
        {
            if(!_health.IsAlive)
                return;

            _timer.Reset();
            _attack.Reset();
            _animator.PlayAnimation(CharacterAction.Hurt, true);
        }

        public void Tick(float deltaTime)
        {
            _timer.Tick(deltaTime);
        }

        public void Disable()
        {
            _health.Damaged -= OnDamaged;
        }
    }
}
