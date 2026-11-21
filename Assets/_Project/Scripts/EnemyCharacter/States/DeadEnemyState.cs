using System;
using Isometric2DGame.Animations;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.StateMachines;

namespace Isometric2DGame.EnemyCharacter.States
{
    public class DeadEnemyState : ITickableState
    {
        private readonly CharacterAnimator _animator;
        private readonly CooldownTimer _expiredTimer;
        private readonly Action _onCorpseExpired;

        private bool _expired;
        
        public DeadEnemyState(EnemyConfig.DeadSettings settings, CharacterAnimator animator, Action onExpired)
        {
            _animator = animator;
            _expiredTimer = new CooldownTimer(settings.CorpseTime);
            _onCorpseExpired = onExpired;
        }
        
        public void Enter()
        {
            _animator.PlayAnimation(CharacterAction.Death);
            _expiredTimer.Reset();
            _expired = false;
        }

        public void Tick(float deltaTime)
        {
            if(_expired)
                return;
            
            _expiredTimer.Tick(deltaTime);
            if (_expiredTimer.IsDone)
            {
                _onCorpseExpired?.Invoke();
                _expired = true;
            }
        }

        public void Exit()
        {
        }
    }
}