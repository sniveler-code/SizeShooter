namespace SizeShooter.Game.States
{
    public sealed class LoseState : GameState
    {
        public LoseState(GameStateMachine stateMachine, GameplayController controller)
            : base(stateMachine, controller) { }

        public override void Enter() => _controller.Model.SetOutcome(GameModel.Outcome.LoseOvercharge);
    }
}
