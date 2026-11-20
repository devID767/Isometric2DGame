using Isometric2DGame.Animations;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.StateMachines;

namespace Isometric2DGame.EnemyCharacter
{

    public class IdleEnemyState : ITickableState
    {
        private readonly CooldownTimer _timer;
        private readonly CharacterAnimator _animator;

        public bool IsDone => _timer.IsDone;

        public IdleEnemyState(float time, CharacterAnimator animator)
        {
            _timer = new CooldownTimer(time);
            _animator = animator;
        }

        public void Enter()
        {
            _timer.Reset();
            _animator.PlayAnimation(CharacterAction.Idle);
        }

        public void Tick(float deltaTime)
        {
            _timer.Tick(deltaTime);
        }

        public void Exit()
        {
        }
    }
}