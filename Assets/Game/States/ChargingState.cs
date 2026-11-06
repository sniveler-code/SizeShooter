namespace SizeShooter.Game.States
{
    public sealed class ChargingState : GameState
    {
        public ChargingState(GameStateMachine stateMachine, GameplayController controller)
            : base(stateMachine, controller)
        {
        }

        public override void Enter()
        {
            _controller.Model.BeginCharge();
            _controller.Input.ChargeEnded += OnChargeEnded;
        }

        public override void Exit() => _controller.Input.ChargeEnded -= OnChargeEnded;

        public override void Update(float deltaTime)
        {
            if (_controller.Model.UpdateCharge(deltaTime))
            {
                _stateMachine.ChangeState<LoseState>();
            }
        }

        private void OnChargeEnded()
        {
            if (_controller.Model.TryReleaseCharge())
                _stateMachine.ChangeState<FiringState>();
            else
                _stateMachine.ChangeState<IdleState>();
        }
    }
}
