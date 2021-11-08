using System.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.States.Base
{
    public interface IStateless
    {
        Task OnEntry();

        Task OnExit();
    }
}