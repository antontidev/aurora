using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public abstract class BaseState<TState, TTrigger> : IState<TTrigger>
        where TTrigger : Enum {
        private readonly Dictionary<TTrigger, Func<Task>> _internalTriggers;

        public bool HasEntry { get; set; }

        public bool HasExit { get; set; }
        
        public bool Entered { get; set; }

        protected BaseState() {
            _internalTriggers = new Dictionary<TTrigger, Func<Task>>();
        }

        public bool HasInternal(TTrigger trigger) => 
            _internalTriggers.ContainsKey(trigger);

        public async Task Internal(TTrigger trigger) {
            var action = _internalTriggers[trigger];

            await action();
        }

        public abstract void RegisterState(StateMachine<TState, TTrigger> stateMachine);

        public async Task TriggerExit() {
            if (!HasExit) return;
            
            await ((IExitState)this).OnExit();
        }

        public async Task TriggerEnter() {
            if (!HasEntry) return;

            await ((IEntryState)this).OnEntry();
        }

        public void InternalTransition(TTrigger trigger, Func<Task> action) => 
            _internalTriggers.Add(trigger, action);
    }
}
