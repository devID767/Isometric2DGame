using Isometric2DGame.Infrastructure;
using Isometric2DGame.StateMachines;

namespace Isometric2DGame.EnemyCharacter
{
    public class IdleEnemyState : ITickableState
    {
        private readonly Timer _timer;

        public bool IsDone => _timer.IsDone;

        public IdleEnemyState(float time)
        {
            _timer = new Timer(time);
        }

        public void Enter()
        {
            _timer.Reset();
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