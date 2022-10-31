using System.Collections.Generic;
using Source.Scripts.Core.StateMachine.States.Base;

namespace Source.Scripts.Core.StateMachine.Configurator.Base {
    public abstract class BaseConfigurator<TState, TTrigger> : IConfigurator<TState, TTrigger>
        where TState : System.Enum
        where TTrigger : System.Enum {
        private readonly List<TTrigger> _reentryList;
        private readonly Dictionary<TTrigger, TState> _transitionMap;
        
        public IState<TTrigger> State { get; }
        public TState StateEnum;

        protected BaseConfigurator(TState state) {
            StateEnum = state;
            _transitionMap = new Dictionary<TTrigger, TState>();
            _reentryList = new List<TTrigger>();
        }
        
        public bool HasTransition(TTrigger trigger) =>
            _transitionMap.ContainsKey(trigger);

        public TState Transition(TTrigger trigger) =>
            _transitionMap[trigger];
        
        public bool HasReentry(TTrigger trigger) =>
            _reentryList.Contains(trigger);

        public IConfigurator<TState, TTrigger> Permit(TTrigger trigger, TState state) {
            _transitionMap.Add(trigger, state);
            return this;
        }

        public IConfigurator<TState, TTrigger> PermitReentry(TTrigger trigger) {
             _reentryList.Add(trigger);
                        
             return this;
        }
    }
}