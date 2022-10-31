using Source.Scripts.Core.StateMachine.Configurator.Base;
using Source.Scripts.Core.StateMachine.States.Base;

namespace Source.Scripts.Core.StateMachine.Configurator {
    public abstract class BaseObjectConfigurator<TState, TTrigger> : BaseConfigurator<TState, TTrigger>
        where TState : System.Enum
        where TTrigger : System.Enum {
        
        public abstract IState<TTrigger> State
        {
            get;
        }

        protected BaseObjectConfigurator(TState state) : base(state) {
        }
    }
}