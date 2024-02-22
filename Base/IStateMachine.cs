using Cysharp.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.Base
{
    public interface IStateMachine<in TTrigger> : IStatelessMachine where TTrigger : System.Enum
    {
        UniTask Fire(TTrigger trigger);
    }
}