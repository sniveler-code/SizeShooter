using System;
using System.Collections.Generic;

namespace SizeShooter.Game.States
{
    public sealed class GameStateMachine
    {
        private readonly Dictionary<Type, GameState> _states = new();
        private GameState _currentState;

        public string CurrentStateName => _currentState != null ? _currentState.GetType().Name : string.Empty;

        public void RegisterState<T>(T state) where T : GameState
        {
            _states[typeof(T)] = state;
        }

        public void ChangeState<T>() where T : GameState
        {
            if (!_states.TryGetValue(typeof(T), out GameState nextState))
                throw new InvalidOperationException($"State {typeof(T).Name} is not registered.");

            _currentState?.Exit();
            _currentState = nextState;
            _currentState.Enter();
        }

        public void Update(float deltaTime) => _currentState?.Update(deltaTime);

        public void Stop()
        {
            _currentState?.Exit();
            _currentState = null;
        }
    }
}
