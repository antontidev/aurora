using System;
using System.Collections.Generic;
using Source.Scripts.Core.StateMachine.Configurator.Base;
using Source.Scripts.Core.StateMachine.States.Base;

namespace Source.Scripts.Core.StateMachine.Configurator
{
    public class Configurator<TState, TTrigger, T> : IConfigurator<TState, TTrigger>
        where TTrigger : Enum
        where T : class, IState<TTrigger>
    {
        public IState<TTrigger> State => _state; 

        public bool Entered { get; set; }
        public bool Exited { get; set; }
        
        private readonly Dictionary<TTrigger, TState> _transitionMap;

        private readonly T _state;
        private readonly List<TTrigger> _reentryList;
        
        public TState StateEnum { get; private set; }

        public Configurator(TState state, T stateObject)
        {
            _transitionMap = new Dictionary<TTrigger, TState>();
            _reentryList = new List<TTrigger>();
            _state = stateObject;
            StateEnum = state;

            _state.HasEntry = stateObject is IEntryState;
            _state.HasExit = stateObject is IExitState;
        }
        
        public Configurator<TState, TTrigger, T> Permit(TTrigger trigger, TState state)
        {
            _transitionMap.Add(trigger, state);

            return this;
        }

        public Configurator<TState, TTrigger, T> PermitReentry(TTrigger trigger)
        {
            _reentryList.Add(trigger);
            
            return this;
        }

        public Configurator<TState, TTrigger, T> SetupInternals(Action<T> action)
        {
            action(_state);

            return this;
        }

        public bool HasReentry(TTrigger trigger) =>
            _reentryList.Contains(trigger);

        public bool HasTransition(TTrigger trigger) =>
            _transitionMap.ContainsKey(trigger);

        public TState Transition(TTrigger trigger) =>
            _transitionMap[trigger];
    }
}
