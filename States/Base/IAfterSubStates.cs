using Cysharp.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public interface IAfterSubStates {
        UniTask OnAfterSubStates();
    }
}