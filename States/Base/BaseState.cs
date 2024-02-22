using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Source.Scripts.Core.StateMachine.States.Base {
    public abstract class BaseState<TState, TTrigger> : IState<TTrigger>
        where TTrigger : Enum {
        private readonly Dictionary<TTrigger, Func<UniTask>> _internalTriggers;

        public bool HasEntry { get; set; }

        public bool HasExit { get; set; }
        
        public bool Entered { get; set; }

        protected BaseState() {
            _internalTriggers = new Dictionary<TTrigger, Func<UniTask>>();
        }

        public bool HasInternal(TTrigger trigger) => 
            _internalTriggers.ContainsKey(trigger);

        public async UniTask Internal(TTrigger trigger) {
            var action = _internalTriggers[trigger];

            await action();
        }

        public abstract void RegisterState(StateMachine<TState, TTrigger> stateMachine);

        public async UniTask TriggerExit() {
            if (!HasExit) return;
            
            await ((IExitState)this).OnExit();
        }

        public async UniTask TriggerEnter() {
            if (!HasEntry) return;

            await ((IEntryState)this).OnEntry();
        }

        public void InternalTransition(TTrigger trigger, Func<UniTask> action) => 
            _internalTriggers.Add(trigger, action);
    }
}
