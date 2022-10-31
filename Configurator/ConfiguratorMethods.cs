using Source.Scripts.Core.StateMachine.Configurator.Base;

namespace Source.Scripts.Core.StateMachine.Configurator {
    public class ConfiguratorMethods<TState, TTrigger> : BaseConfigurator<TState, TTrigger> 
        where TState : System.Enum
        where TTrigger : System.Enum {
        public ConfiguratorMethods(TState state) : base(state) {
        }
    }
}