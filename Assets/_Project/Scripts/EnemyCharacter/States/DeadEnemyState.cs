using Isometric2DGame.Animations;
using Isometric2DGame.StateMachines;

namespace Isometric2DGame.EnemyCharacter
{
    public class DeadEnemyState : IState
    {
        private readonly CharacterAnimator _animator;
        
        public DeadEnemyState(CharacterAnimator animator)
        {
            _animator = animator;
        }
        
        public void Enter()
        {
            _animator.PlayAnimation(CharacterAction.Death);
        }

        public void Exit()
        {
        }
    }
}