using System;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public interface ISubState<TState, TTrigger>
        where TState : Enum
        where TTrigger : Enum {
        TState RootState { get; }
    }
}
