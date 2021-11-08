using System.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.States.Base
{
    public interface IState<in TTrigger> : IStateless
    {
        bool HasInternal(TTrigger trigger);

        Task Internal(TTrigger trigger);
    }
}