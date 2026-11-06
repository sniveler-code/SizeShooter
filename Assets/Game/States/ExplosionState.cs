using UnityEngine;

namespace SizeShooter.Game.States
{
    public sealed class ExplosionState : GameState
    {
        private float _elapsed;
        private float _duration;

        public ExplosionState(GameStateMachine stateMachine, GameplayController controller)
            : base(stateMachine, controller)
        {
        }

        public override void Enter()
        {
            _controller.Model.ClearShot();
            _duration = Mathf.Max(0.01f, _controller.Settings.ExplosionDuration);
            _elapsed = 0f;
        }

        public override void Update(float deltaTime)
        {
            _elapsed += deltaTime;

            if (_elapsed >= _duration)
                _stateMachine.ChangeState<MovingState>();
        }
    }
}
