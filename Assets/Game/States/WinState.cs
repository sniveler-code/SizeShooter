namespace SizeShooter.Game.States
{
    public sealed class WinState : GameState
    {
        public WinState(GameStateMachine stateMachine, GameplayController controller)
            : base(stateMachine, controller) { }

        public override void Enter() => _controller.Model.SetOutcome(GameModel.Outcome.Win);
    }
}
