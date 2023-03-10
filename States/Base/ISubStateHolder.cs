using System;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public interface ISubStateHolder<TState, TTrigger>
        where TState : Enum
        where TTrigger : Enum {
        void RegisterSubState<TStateConcrete>(TState state, TState subState, TStateConcrete stateConcrete)
            where TStateConcrete : BaseState<TState, TTrigger>;

        void UnRegisterSubState(TState state, TState subState);
    }
}