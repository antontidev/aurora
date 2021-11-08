using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Source.Scripts.Core.StateMachine.Configurator.Base;
using Source.Scripts.Core.StateMachine.States.Base;

namespace Source.Scripts.Core.StateMachine.Configurator
{
    public class Configurator<TState, TTrigger> : IConfigurator 
        where TState : Enum
        where TTrigger : Enum
    {
        private readonly Dictionary<TTrigger, TState> _transitionMap;
        
        public readonly IState<TTrigger> State;
        private readonly List<TTrigger> _reentryList;
        
        public TState StateEnum;

        public Configurator(TState state, IState<TTrigger> stateObject)
        {
            _transitionMap = new Dictionary<TTrigger, TState>();
            _reentryList = new List<TTrigger>();
            State = stateObject;
            StateEnum = state;
        }
        
        public Configurator<TState, TTrigger> Permit(TTrigger trigger, TState state)
        {
            _transitionMap.Add(trigger, state);

            return this;
        }

        public Configurator<TState, TTrigger> PermitReentry(TTrigger trigger)
        {
            _reentryList.Add(trigger);
            
            return this;
        }

        public bool HasReentry(TTrigger trigger) =>
            _reentryList.Contains(trigger);

        public async Task Reentry()
        {
            await State.OnExit();
            await State.OnEntry();
        }

        public bool HasTransition(TTrigger trigger) =>
            _transitionMap.ContainsKey(trigger);

        public TState Transition(TTrigger trigger) =>
            _transitionMap[trigger];
    }
}