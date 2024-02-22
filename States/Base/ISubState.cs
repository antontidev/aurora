namespace Source.Scripts.Core.StateMachine.States.Base {
    public interface ISubState<TState> {
        TState RootState { get; }
    }
}
