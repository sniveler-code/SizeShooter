namespace SizeShooter.Game.States
{
    public sealed class FiringState : GameState
    {
        public FiringState(GameStateMachine stateMachine, GameplayController controller)
            : base(stateMachine, controller)
        {
        }

        public override void Update(float deltaTime)
        {
            _controller.Model.UpdateShot(deltaTime);

            if (!_controller.HasCurrentObstacle)
            {
                _stateMachine.ChangeState<MovingState>();
                return;
            }

            float shotRadius = _controller.Model.ShotSize * 0.5f;
            float obstacleDistance = _controller.GetDistanceAlongTrack(_controller.CurrentObstacle.position);
            float shotFront = _controller.Model.ShotPosition + shotRadius;

            if (!(shotFront >= obstacleDistance)) return;

            _controller.ExplodeAt(obstacleDistance, shotRadius);
            _controller.PlayExplosionAt(obstacleDistance, shotRadius);
            _stateMachine.ChangeState<ExplosionState>();
        }
    }
}
