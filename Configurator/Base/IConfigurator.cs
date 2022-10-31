namespace Source.Scripts.Core.StateMachine.Configurator.Base
{
    public interface IConfigurator<TState, TTrigger> where TState : System.Enum where TTrigger : System.Enum
    {
        bool HasTransition(TTrigger trigger);
        TState Transition(TTrigger trigger);
        bool HasReentry(TTrigger trigger);
        IConfigurator<TState, TTrigger> Permit(TTrigger trigger, TState state);
        IConfigurator<TState, TTrigger> PermitReentry(TTrigger trigger);
    }
}
