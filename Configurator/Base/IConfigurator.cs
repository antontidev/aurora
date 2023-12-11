using Source.Scripts.Core.StateMachine.States.Base;
using System.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.Configurator.Base
{
    public interface IConfigurator<TState, TTrigger> where TTrigger : System.Enum
    {
        IState<TTrigger> State
        {
            get;
        }
        bool Entered { get; set; }
        bool Exited { get; set; }
        TState StateEnum { get; }
        bool HasTransition(TTrigger trigger);
        TState Transition(TTrigger trigger);
        bool HasReentry(TTrigger trigger);
    }
}