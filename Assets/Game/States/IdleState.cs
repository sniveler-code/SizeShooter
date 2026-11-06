namespace SizeShooter.Game.States
{
    public sealed class IdleState : GameState
    {
        public IdleState(GameStateMachine stateMachine, GameplayController controller)
            : base(stateMachine, controller) { }

        public override void Enter() => _controller.Input.ChargeStarted += OnChargeStarted;
        public override void Exit() => _controller.Input.ChargeStarted -= OnChargeStarted;
        private void OnChargeStarted() => _stateMachine.ChangeState<ChargingState>();
    }
}
