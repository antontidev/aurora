using System;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public interface ISubState<in TStateConcrete, in TState, TTrigger> 
        where TStateConcrete : BaseState<TTrigger> 
        where TTrigger : Enum {
        void RegisterSubState(TState stateEnum);
        
        void UnRegisterSubState(TState stateEnum);
    }
}