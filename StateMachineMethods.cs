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

        public StateMachineMethods<TState, TTrigger> DefineState(TState key) {


            return this;
        }

        public void RegisterStateCircle(TState state, Queue<TTrigger> triggerQueue) {
            
        }

        public StateMachineMethods<TState, TTrigger> RegisterStateOrder(TState first, TState second) {

            return this;
        }

        public StateMachineMethods<TState, TTrigger> RegisterSubState(TState root, TState child) {

            return this;
        }

        public StateMachineMethods<TState, TTrigger> PermitOnlyOnState(TState toTrigger, TState toPermit) {
            return this;
        }

        public StateMachineMethods<TState, TTrigger> AllowMultiple(TState state) {
            return this;
        }

        public StateMachineMethods<TState, TTrigger> PermitRootFromChild(TState child, TTrigger trigger) {
            return this;
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