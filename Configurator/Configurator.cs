using System;
using Source.Scripts.Core.StateMachine.Configurator.Base;
using Source.Scripts.Core.StateMachine.States.Base;

namespace Source.Scripts.Core.StateMachine.Configurator
{
    public class Configurator<TState, TTrigger, T> : BaseObjectConfigurator<TState, TTrigger>
        where TState : Enum
        where TTrigger : Enum
        where T : class, IState<TTrigger>
    {
        public override IState<TTrigger> State => _state;

        private readonly T _state;

        public Configurator(TState state, T stateObject) : base(state)
        {
            _state = stateObject;

            _state.HasEntry = stateObject is IEntryState;
            _state.HasExit = stateObject is IExitState;
        }
        
        public Configurator<TState, TTrigger, T> SetupInternals(Action<T> action)
        {
            action(_state);

            return this;
        }
    }
}
