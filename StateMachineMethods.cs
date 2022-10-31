using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Source.Scripts.Core.StateMachine.Base;
using Source.Scripts.Core.StateMachine.Configurator.Base;

namespace Source.Scripts.Core.StateMachine {
    public class StateMachineMethods<TState, TTrigger> : BaseStateMachine<TState, TTrigger>
        where TTrigger : System.Enum 
        where TState : System.Enum {
        private Dictionary<TState, Action> _entryMethods;
        private Dictionary<TState, Action> _exitMethods;
        
        public StateMachineMethods(TState start) : base(start) {
            _entryMethods = new Dictionary<TState, Action>();
            _exitMethods = new Dictionary<TState, Action>();
        }

        public override IConfigurator<TState, TTrigger> CurrentState { get; }

        protected override async Task InternalFire(TTrigger trigger) {
            
        }

        public void RegisterState(TState key) {
            
        }

        public void RegisterEntryMethodFor(TState state, Action method) {
            _entryMethods.Add(state, method);
        }

        public void RegisterExitMethodFor(TState state, Action method) {
            _exitMethods.Add(state, method);
        }

        protected override async Task ExitState() {
            
        }

        protected override async Task EntryState() {
        }
    }
}