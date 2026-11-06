namespace SizeShooter.Game.States
{
    public sealed class MovingState : GameState
    {
        private bool _movingThroughDoor;

        public MovingState(GameStateMachine stateMachine, GameplayController controller)
            : base(stateMachine, controller)
        {
        }

        public override void Enter()
        {
            _movingThroughDoor = !_controller.HasCurrentObstacle;
            float stopPosition = _controller.GetNextStopPosition();
            _controller.Model.StartMoving(stopPosition);
        }

        public override void Update(float deltaTime)
        {
            if (!_controller.Model.UpdateMoving(deltaTime)) return;

            if (_movingThroughDoor)
                _stateMachine.ChangeState<WinState>();
            else
                _stateMachine.ChangeState<IdleState>();
        }
    }
}
