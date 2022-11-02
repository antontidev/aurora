using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Source.Scripts.Core.StateMachine.Configurator.Base;

namespace Source.Scripts.Core.StateMachine.Base {
    public abstract class BaseStateMachine<TState, TTrigger> : IStateMachine<TTrigger>
        where TTrigger : System.Enum where TState : System.Enum{
        protected TState CurrentStateEnum;
        protected readonly Dictionary<TState, TState> AutoTransitionDictionary;

        public abstract IConfigurator<TState, TTrigger> CurrentState {
            get;
        }
        
        public TTrigger CurrentTrigger { get; private set; }

        protected BaseStateMachine(TState start) {
            CurrentStateEnum = start;
            AutoTransitionDictionary = new Dictionary<TState, TState>();
        }

        public async Task Fire(TTrigger trigger) {
            async void Action() => await InternalFire(trigger);
            
            await Create(Action);
        }

        protected abstract Task InternalFire(TTrigger trigger);

        protected abstract Task ExitState();

        protected abstract Task EntryState();

        protected virtual async Task CheckAutoTransition()
        {
            if (AutoTransitionDictionary.ContainsKey(CurrentStateEnum))
            {
                var nextState = AutoTransitionDictionary[CurrentStateEnum];

                await ExitState();

                CurrentStateEnum = nextState;
                await EntryState();
                await CheckAutoTransition();
            }
        }
        
        private static async Task Create(Action action)
        {
            await Task.Factory.StartNew(action,
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}