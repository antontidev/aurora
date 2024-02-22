using Cysharp.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public interface IState<in TTrigger> : IStateless {
        bool HasInternal(TTrigger trigger);
        public bool HasEntry { get; set; }
        public bool HasExit { get; set; }
        UniTask Internal(TTrigger trigger);
        UniTask TriggerExit();
        UniTask TriggerEnter();
    }
}