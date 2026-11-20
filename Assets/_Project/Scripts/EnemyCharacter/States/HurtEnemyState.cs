using Isometric2DGame.Animations;
using Isometric2DGame.Combat;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.StateMachines;

namespace Isometric2DGame.EnemyCharacter
{
    public class HurtEnemyState : ITickableState
    {
        private readonly EnemyConfig.HurtSettings _settings;
        private readonly CharacterAnimator _animator;
        private readonly Health _health;

        private readonly CooldownTimer _durationTimer;

        public bool IsTriggered { get; private set; }
        public bool IsDone => _durationTimer.IsDone;

        public HurtEnemyState(EnemyConfig.HurtSettings settings, CharacterAnimator animator, Health health)
        {
            _settings = settings;
            _animator = animator;
            _health = health;

            _durationTimer = new CooldownTimer(_settings.Duration);
            _animator.SetDuration(CharacterAction.Hurt, _settings.Duration);

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

        private void OnDamaged()
        {
            if(_health.IsAlive)
                IsTriggered = true;
            else IsTriggered = false;
        }
    }
}