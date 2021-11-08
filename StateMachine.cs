using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Source.Scripts.Core.StateMachine.Base;
using Source.Scripts.Core.StateMachine.Configurator;
using Source.Scripts.Core.StateMachine.States.Base;
using UnityEngine;

namespace Source.Scripts.Core.StateMachine
{
    public class StateMachine<TState, TTrigger> : IStateMachine<TTrigger>
        where TTrigger : Enum where TState : Enum
    {
        private readonly Dictionary<TState, Configurator<TState, TTrigger>> _states;
        private readonly Dictionary<TState, TState> _autoTransition;

        private TState _currentState;

        public StateMachine(TState start)
        {
            _currentState = start;
            _states = new Dictionary<TState, Configurator<TState, TTrigger>>();
            _autoTransition = new Dictionary<TState, TState>();
        }

        public async Task Fire(TTrigger trigger)
        {
            var configurator = _states[_currentState];
            var state = configurator.State;

            if (state.HasInternal(trigger))
            {
                await state.Internal(trigger);
                return;
            }

            if (configurator.HasReentry(trigger))
            {
                await configurator.Reentry();
                return;
            }
            
            if (configurator.HasTransition(trigger))
            {
                await state.OnExit();

                _currentState = configurator.Transition(trigger);

                configurator = _states[_currentState];

                state = configurator.State;
                
                await state.OnEntry();

                await CheckAutoTransition();
            }
            else
            {
                Debug.LogError($"Trigger {trigger} for state {nameof(_currentState)} and for whole FSM doesn't registered!");
            }
        }

        private async Task CheckAutoTransition()
        {
            if (_autoTransition.ContainsKey(_currentState))
            {
                var configurator = _states[_currentState];
                var state = configurator.State;

                var nextState = _autoTransition[_currentState];

                await state.OnExit();

                _currentState = nextState;
                state = _states[nextState].State;
                await state.OnEntry();
            }
        }
        
        public async Task Start()
        {
            var  state = _states[_currentState].State;

            await state.OnEntry();

            await CheckAutoTransition();
        }

        public Configurator<TState, TTrigger> RegisterState<T>(TState key, T state) 
            where T : BaseState<TTrigger>
        {
            var configurator = new Configurator<TState, TTrigger>(key, state);
            _states.Add(key, configurator);
            state.Configure();

            return configurator;
        }

        public StateMachine<TState, TTrigger> AutoTransition(TState oldState, TState newState)
        {
            _autoTransition.Add(oldState, newState);

            return this;
        }
    }
}