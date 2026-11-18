namespace Isometric2DGame.StateMachines
{
    public interface IState
    {
        public void Enter();
        public void Exit();
    }

    public interface ITickableState : IState
    {
        public void Tick(float deltaTime);
    }

    public interface IFixedTickableState : IState
    {
        public void FixedTick();
    }
}