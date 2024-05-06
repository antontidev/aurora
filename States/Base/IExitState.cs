using System.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public interface IExitState {
        Task OnExit();
    }
}