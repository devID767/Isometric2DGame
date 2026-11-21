using System;

namespace Isometric2DGame.StateMachines
{
    public readonly struct StateTransition
    {
        public IState To { get; }
        public Func<bool> Condition { get; }

        public bool CanTransitionToSelf { get; }

        public StateTransition(IState to, Func<bool> confition, bool canTransitionToSlef = false)
        {
            To = to;
            Condition = confition;
            CanTransitionToSelf = canTransitionToSlef;
        }
    }
}