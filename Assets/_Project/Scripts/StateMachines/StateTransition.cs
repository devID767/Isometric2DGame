using System;

namespace Isometric2DGame.StateMachines
{
    public readonly struct StateTransition
    {
        public IState To { get; }
        public Func<bool> Condition { get; }

        public StateTransition(IState to, Func<bool> confition)
        {
            To = to;
            Condition = confition;
        }
    }
}