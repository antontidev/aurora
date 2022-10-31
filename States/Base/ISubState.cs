using System;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public interface ISubState<TState>
        where TState : Enum {
        TState RootState { get; }
    }
}
