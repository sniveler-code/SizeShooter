namespace SizeShooter.Game.States
{
    public abstract class GameState
    {
        protected readonly GameStateMachine _stateMachine;
        protected readonly GameplayController _controller;

        protected GameState(GameStateMachine stateMachine, GameplayController controller)
        {
            _stateMachine = stateMachine;
            _controller = controller;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Update(float deltaTime)
        {
        }
    }
}
