using System.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.States.Base
{
    public interface IState<in TTrigger> : IStateless
    {
        bool HasInternal(TTrigger trigger);
        
        public bool HasEntry { get; set; }
        
        public bool HasExit { get; set; }
        
        Task Internal(TTrigger trigger);

        Task TriggerExit();

        Task TriggerEnter();
    }
}