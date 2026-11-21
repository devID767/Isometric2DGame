using System;
using System.Collections.Generic;

namespace Isometric2DGame.StateMachines
{
    public class StateMachine
    {
        private readonly Dictionary<IState, List<StateTransition>> _transitions;
        private readonly List<StateTransition> _anyTransitions;

        private readonly IState _initialState;

        public IState CurrentState { get; private set; }

        public StateMachine(IState initialState)
        {
            _transitions = new Dictionary<IState, List<StateTransition>>();
            _anyTransitions = new List<StateTransition>();
            _initialState = initialState;
        }

        public void Restart()
        {
            Stop();
            ChangeState(_initialState);
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            if (!_transitions.ContainsKey(from))
                _transitions.Add(from, new List<StateTransition>());

            _transitions[from].Add(new StateTransition(to, condition));
        }

        public void AddAnyTransition(IState to, Func<bool> condition, bool canTransitionToSelf = false)
        {
            _anyTransitions.Add(new StateTransition(to, condition, canTransitionToSelf));
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

            if (TryTransition(_anyTransitions))
                return;

            if (_transitions.TryGetValue(CurrentState, out var transitions))
            {
                if(TryTransition(transitions))
                    return;
            }

            if (CurrentState is ITickableState tickableState)
                tickableState.Tick(deltaTime);
        }

        private bool TryTransition(List<StateTransition> transitions)
        {
            foreach (var transition in transitions)
            {
                var isSelf = transition.To == CurrentState;
                if (isSelf && !transition.CanTransitionToSelf)
                    continue;

                if (!transition.Condition())
                    continue;

                if (isSelf)
                {
                    CurrentState.Exit();
                    CurrentState.Enter();
                }
                else
                {
                    ChangeState(transition.To);
                }
                return true;
            }
            return false;
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