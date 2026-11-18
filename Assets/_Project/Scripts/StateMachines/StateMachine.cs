using System;
using System.Collections.Generic;

namespace Isometric2DGame.StateMachines
{
    public class StateMachine
    {
        private readonly Dictionary<IState, List<StateTransition>> _transitions;

        private readonly IState _initialState;

        public IState CurrentState { get; private set; }

        public StateMachine(IState initialState)
        {
            _transitions = new Dictionary<IState, List<StateTransition>>();
            _initialState = initialState;
        }

        public void Restart()
        {
            Stop();
            CurrentState = _initialState;
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            if (!_transitions.ContainsKey(from))
                _transitions.Add(from, new List<StateTransition>());

            _transitions[from].Add(new StateTransition(to, condition));
        }

        public void ChangeState(IState state)
        {
            if (state == CurrentState)
                return;

            if (CurrentState != null)
                CurrentState.Exit();

            CurrentState = state;
            CurrentState.Enter();
        }

        public void Tick(float deltaTime)
        {
            if (CurrentState == null)
                return;

            if (_transitions.TryGetValue(CurrentState, out var transitions))
            {
                foreach (var transition in transitions)
                {
                    if (transition.Condition())
                    {
                        ChangeState(transition.To);
                        return;
                    }
                }
            }

            if (CurrentState is ITickableState tickableState)
                tickableState.Tick(deltaTime);
        }

        public void FixedTick()
        {
            if (CurrentState == null)
                return;

            if (CurrentState is IFixedTickableState fixedTickableState)
                fixedTickableState.FixedTick();
        }

        public void Stop()
        {
            CurrentState?.Exit();
            CurrentState = null;
        }
    }
}