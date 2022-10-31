using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Source.Scripts.Core.StateMachine.Base;
using Source.Scripts.Core.StateMachine.Configurator;
using Source.Scripts.Core.StateMachine.Configurator.Base;
using Source.Scripts.Core.StateMachine.States.Base;
using UnityEngine;

namespace Source.Scripts.Core.StateMachine
{
    public class StateMachine<TState, TTrigger> : BaseStateMachine<TState, TTrigger>
        where TTrigger : Enum where TState : Enum
    {
        private readonly Dictionary<TState, BaseObjectConfigurator<TState, TTrigger>> _states;
        private readonly Dictionary<TState, List<BaseObjectConfigurator<TState, TTrigger>>> _subStates;

        public StateMachine(TState start) : base(start)
        {
            _states = new Dictionary<TState, BaseObjectConfigurator<TState, TTrigger>>();
            _subStates = new Dictionary<TState, List<BaseObjectConfigurator<TState, TTrigger>>>();
        }

        public override IConfigurator<TState, TTrigger> CurrentState => _states[CurrentStateEnum];

        protected override async Task ExitState() {
            var configurator = _states[CurrentStateEnum];
            var state = configurator.State;

            await state.TriggerExit();

            if (_subStates.ContainsKey(CurrentStateEnum))
            {
                var subStatesList = _subStates[CurrentStateEnum];

                foreach (var subState in subStatesList)
                {
                    await subState.State.TriggerExit();
                }
            }
        }

        protected override async Task EntryState() {
            var state = _states[CurrentStateEnum].State;
            if (_subStates.ContainsKey(CurrentStateEnum))
            {
                if (state is IBeforeSubStates beforeSubStates) {
                    await beforeSubStates.OnBeforeSubStates();
                }
                
                var subStatesList = _subStates[CurrentStateEnum];

                foreach (var subState in subStatesList)
                {
                    await subState.State.TriggerEnter();
                }

                if (state is IAfterSubStates afterSubStates) {
                    await afterSubStates.OnAfterSubStates();
                }
            }
            
            await state.TriggerEnter();
        }

        protected override async Task InternalFire(TTrigger trigger) {
            var configurator = _states[CurrentStateEnum];
            var state = configurator.State;

            if (state.HasInternal(trigger))
            {
                await state.Internal(trigger);
                return;
            }

            if (configurator.HasReentry(trigger)) 
            {
                await ExitState();
                await EntryState();
                return;
            }

            if (configurator.HasTransition(trigger))
            {
                await ExitState();
                
                CurrentStateEnum = configurator.Transition(trigger);

                await EntryState();
                await CheckAutoTransition();
            }
            else
            {
                Debug.LogError($"Trigger {trigger} for state {nameof(CurrentStateEnum)} and for whole FSM doesn't registered!");
            }
        }


        public Configurator<TState, TTrigger, T> RegisterSubStateFor<T>(TState stateKey, TState subStateKey, T subState)
            where T : BaseState<TTrigger>
        {
            var subStateConfigurator = new Configurator<TState, TTrigger, T>(subStateKey, subState);
            _states.Add(subStateKey, subStateConfigurator);

            List<BaseObjectConfigurator<TState, TTrigger>> subStatesList;
            if (_subStates.ContainsKey(stateKey))
            {
                subStatesList = _subStates[stateKey];
            }
            else
            {
                subStatesList = new List<BaseObjectConfigurator<TState, TTrigger>>();
                _subStates.Add(stateKey, subStatesList);
            }
            subStatesList.Add(subStateConfigurator);
            return subStateConfigurator;
        }
        
        public void UnRegisterSubStateFor(TState stateKey, TState subStateKey) {
            var subStateConfigurator = _states[subStateKey];
         
            if (_subStates.ContainsKey(stateKey))
            {
                var subStatesList = _subStates[stateKey];
                subStatesList.Remove(subStateConfigurator);

                if (subStatesList.Count < 1) {
                    _subStates.Remove(stateKey);
                }
            }

            _states.Remove(subStateKey);
        }

        public Configurator<TState, TTrigger, T> RegisterState<T>(TState key, T state) 
            where T : BaseState<TTrigger>
        {
            var configurator = new Configurator<TState, TTrigger, T>(key, state);
            _states.Add(key, configurator);

            return configurator;
        }

        public void UnregisterState(TState state) 
        {
            _states.Remove(state);
        }

        public StateMachine<TState, TTrigger> AutoTransition(TState oldState, TState newState)
        {
            AutoTransitionDictionary.Add(oldState, newState);

            return this;
        }

        public async Task Start()
        {
            await EntryState();
            await CheckAutoTransition();
        }

        public async Task ForceExit()
        {
            await ExitState();
        }
    }
}
