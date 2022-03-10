using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.States.Base
{
    public abstract class BaseState<TTrigger> : IState<TTrigger>
        where TTrigger : Enum
    {
        private readonly Dictionary<TTrigger, Func<Task>> _internalTriggers;

        public bool HasEntry { get; set; }

        public bool HasExit { get; set; }

        protected BaseState()
        {
            _internalTriggers = new Dictionary<TTrigger, Func<Task>>();
        }

        public bool HasInternal(TTrigger trigger) => 
            _internalTriggers.ContainsKey(trigger);

        public async Task Internal(TTrigger trigger)
        {
            var action = _internalTriggers[trigger];

            await action();
        }

        public async Task TriggerExit()
        {
            if (HasExit)
                await ((IExitState)this).OnExit();
        }

        public async Task TriggerEnter()
        {
            if (HasExit)
                await ((IEntryState)this).OnEntry();
        }

        public void InternalTransition(TTrigger trigger, Func<Task> action) => 
            _internalTriggers.Add(trigger, action);
    }
}
