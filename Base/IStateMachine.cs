using System.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.Base
{
    public interface IStateMachine<in TTrigger> : IStatelessMachine where TTrigger : System.Enum
    {
        Task Fire(TTrigger trigger);
    }
}