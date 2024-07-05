using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Source.Scripts.Core.StateMachine.Base;
using Source.Scripts.Core.StateMachine.Configurator;
using Source.Scripts.Core.StateMachine.Configurator.Base;
using Source.Scripts.Core.StateMachine.States.Base;
using UnityEngine;

namespace Source.Scripts.Core.StateMachine {
    public class StateMachine<TState, TTrigger> : IStateMachine<TTrigger>
        where TTrigger : Enum {
        private readonly Dictionary<TState, IConfigurator<TState, TTrigger>> _states;
        private readonly Dictionary<TState, TState> _autoTransition;
        private readonly Dictionary<TState, List<IConfigurator<TState, TTrigger>>> _subStates;

        private TState _currentState;

        private List<Task> _tasks;

        public IConfigurator<TState, TTrigger> CurrentState => _states[_currentState];

        public StateMachine(TState start) {
            _currentState = start;
            _states = new Dictionary<TState, IConfigurator<TState, TTrigger>>();
            _autoTransition = new Dictionary<TState, TState>();
            _subStates = new Dictionary<TState, List<IConfigurator<TState, TTrigger>>>();
            _tasks = new List<Task>();
        }

        public Configurator<TState, TTrigger, T> GetConfigurator<T>(TState key) where T : class, IState<TTrigger> {
            return (Configurator<TState, TTrigger, T>)_states[key];
        }

        public async Task Fire(TTrigger trigger) {
            async Task Action() => await InternalFire(trigger);

            await Create(Action);
        }

        private async Task InternalFire(TTrigger trigger) {
            var configurator = _states[_currentState];
            var state = configurator.State;

            if (_subStates.ContainsKey(configurator.StateEnum)) {
                var subStatesList = _subStates[configurator.StateEnum];

                foreach (var subState in subStatesList) {
                    if (subState.State.HasInternal(trigger)) {
                        var internalTask = subState.State.Internal(trigger);
                        _tasks.Add(internalTask);
                    }
                }

                if (_tasks.Count != 0) {
                    await Task.WhenAll(_tasks);
                    _tasks.Clear();
                }
            }

            if (state.HasInternal(trigger)) {
                await state.Internal(trigger);
                return;
            }

            if (_subStates.ContainsKey(configurator.StateEnum)) {
                var subStates = _subStates[configurator.StateEnum];

                bool containsInChild = false;
                foreach (var stateConfigurator in subStates) {
                    var subState = stateConfigurator.State;

                    if (subState.HasInternal(trigger)) {
                        containsInChild = true;
                        var internalTask = subState.Internal(trigger);
                        _tasks.Add(internalTask);
                    }
                }

                if (containsInChild) {
                    await Task.WhenAll(_tasks);
                    _tasks.Clear();
                    return;
                }
            }
            
            if (configurator.HasReentry(trigger)) {
                await ExitState();
                await EntryState();
                return;
            }

            if (configurator.HasTransition(trigger)) {
                await ExitState();
                
                _currentState = configurator.Transition(trigger);

                await EntryState();
                await CheckAutoTransition();
            }
            else {
                Debug.LogWarning($"Trigger {trigger} for state {nameof(_currentState)} and for whole FSM doesn't registered!");
            }
        }

        public Configurator<TState, TTrigger, T> RegisterSubStateFor<T>(TState stateKey, TState subStateKey, T subState)
            where T : BaseState<TState, TTrigger> {
            var subStateConfigurator = new Configurator<TState, TTrigger, T>(subStateKey, subState);
            _states[subStateKey] = subStateConfigurator;

            List<IConfigurator<TState, TTrigger>> subStatesList;
            if (_subStates.ContainsKey(stateKey)) {
                subStatesList = _subStates[stateKey];
            }
            else {
                subStatesList = new List<IConfigurator<TState, TTrigger>>();
                _subStates.Add(stateKey, subStatesList);
            }
            subStatesList.Add(subStateConfigurator);
            
            if (_states.ContainsKey(stateKey)) {
                subStateConfigurator.Entered = true;
                Create(subState.TriggerEnter);
            }
            
            return subStateConfigurator;
        }
        
        public void UnRegisterSubStateFor(TState stateKey, TState subStateKey) {
            var subStateConfigurator = _states[subStateKey];
         
            if (_subStates.ContainsKey(stateKey)) {
                var rootState = _states[stateKey];
                if (!rootState.Exited) {
                    Create(subStateConfigurator.State.TriggerExit);
                }
                
                var subStatesList = _subStates[stateKey];
                subStatesList.Remove(subStateConfigurator);

                if (subStatesList.Count < 1) {
                    _subStates.Remove(stateKey);
                }
            }

            _states.Remove(subStateKey);
        }

        public Configurator<TState, TTrigger, T> RegisterState<T>(TState key, T state) 
            where T : BaseState<TState, TTrigger> {
            if (_states.ContainsKey(key)) {
                _states.Remove(key);
            }
            var configurator = new Configurator<TState, TTrigger, T>(key, state);
            _states.Add(key, configurator);

            return configurator;
        }

        public void UnregisterState(TState state) {
            _states.Remove(state);
        }

        public StateMachine<TState, TTrigger> AutoTransition(TState oldState, TState newState) {
            _autoTransition.Add(oldState, newState);

            return this;
        }

        public async Task Start() {
            await EntryState();
            await CheckAutoTransition();
        }

        public async Task ForceExit() {
            await ExitState();
        }
        
        private async Task ExitState() { 
            var configurator = _states[_currentState];
            var state = configurator.State;

            configurator.Entered = false;
            configurator.Exited = true;
            await state.TriggerExit();

            if (_subStates.ContainsKey(_currentState)) {
                var subStatesList = _subStates[_currentState];

                foreach (var subStateConfigurator in subStatesList) {
                    subStateConfigurator.Entered = false;
                    subStateConfigurator.Exited = true;
                    var exitTask = subStateConfigurator.State.TriggerExit();
                    _tasks.Add(exitTask);
                }

                if (_tasks.Count != 0) {
                    await Task.WhenAll(_tasks);
                    _tasks.Clear();
                }
            }
        }
        
        private async Task EntryState() {
            var stateConfigurator = _states[_currentState];
            var state = stateConfigurator.State;
            stateConfigurator.Entered = true;
            if (_subStates.ContainsKey(_currentState)) {
                if (state is IBeforeSubStates beforeSubStates) {
                    await beforeSubStates.OnBeforeSubStates();
                }
                
                var subStatesList = _subStates[_currentState].ToList();

                foreach (var subStateConfigurator in subStatesList) {
                    if (!subStateConfigurator.Entered) {
                        subStateConfigurator.Entered = true;
                        var enterTask = subStateConfigurator.State.TriggerEnter();
                        _tasks.Add(enterTask);
                    }
                }
                if (_tasks.Count != 0) {
                    await Task.WhenAll(_tasks);
                    _tasks.Clear();
                }

                if (state is IAfterSubStates afterSubStates) {
                    await afterSubStates.OnAfterSubStates();
                }

                foreach (var subStateConfigurator in subStatesList) {
                    if (subStateConfigurator.State is IAfterSubStates subStateAfter) {
                        var afterSubStatesTask = subStateAfter.OnAfterSubStates();
                        _tasks.Add(afterSubStatesTask);
                    }
                }
                
                if (_tasks.Count != 0) {
                    await Task.WhenAll(_tasks);
                    _tasks.Clear();
                }
            }
            
            await state.TriggerEnter();
        }

        private async Task CheckAutoTransition() {
            if (_autoTransition.ContainsKey(_currentState)) {
                var nextState = _autoTransition[_currentState];

                await ExitState();

                _currentState = nextState;
                await EntryState();
                await CheckAutoTransition();
            }
        }

        private static async Task Create(Func<Task> action) {
            var task = Task.Factory.StartNew(action,
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());

            await task.Unwrap();
        }
    }
}
