using System;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public interface ISubStateHolder<in TState, TTrigger>
        where TState : Enum
        where TTrigger : Enum {
        void RegisterSubState<TStateConcrete>(TState state, TState subState, TStateConcrete stateConcrete)
            where TStateConcrete : BaseState<TTrigger>;

        void UnRegisterSubState(TState state);
    }
}