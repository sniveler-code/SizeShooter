using System.Collections.Generic;
using System.Linq;
using SizeShooter.Game.States;
using SizeShooter.Game.View;
using SizeShooter.Input;
using UnityEngine;

namespace SizeShooter.Game
{
    [DisallowMultipleComponent]
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private LevelSettings settings;
        [SerializeField] private PlayerInput input;
        [SerializeField] private GameView view;

        private GameModel _model;
        private GameStateMachine _stateMachine;
        private Transform[] _obstacles;
        private int _obstacleIndex;

        public LevelSettings Settings => settings;
        public PlayerInput Input => input;
        public GameModel Model => _model;
        public string StateName => _stateMachine != null ? _stateMachine.CurrentStateName : string.Empty;

        public float DistanceToDoor =>
            view == null || _model == null ? 0f : GetDistanceAlongTrack(view.DoorPosition) - _model.PlayerPosition;

        public bool HasCurrentObstacle => _obstacles != null && _obstacleIndex < _obstacles.Length - 1;
        public Transform CurrentObstacle => _obstacles[_obstacleIndex];

        private void Awake()
        {
            _model = new GameModel(settings);
            view.Init(_model);

            _stateMachine = new GameStateMachine();
            _stateMachine.RegisterState(new IdleState(_stateMachine, this));
            _stateMachine.RegisterState(new MovingState(_stateMachine, this));
            _stateMachine.RegisterState(new ChargingState(_stateMachine, this));
            _stateMachine.RegisterState(new FiringState(_stateMachine, this));
            _stateMachine.RegisterState(new ExplosionState(_stateMachine, this));
            _stateMachine.RegisterState(new WinState(_stateMachine, this));
            _stateMachine.RegisterState(new LoseState(_stateMachine, this));
        }

        private void Start()
        {
            CollectObstacles();
            _stateMachine.ChangeState<MovingState>();
        }

        private void OnDestroy() => _stateMachine?.Stop();

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
            view.Render(Time.deltaTime);
        }

        private void CollectObstacles()
        {
            var hits = new RaycastHit[100];
            float distance = Vector3.Distance(view.PlayerPosition, view.DoorPosition);
            int size = Physics.SphereCastNonAlloc(
                view.PlayerPosition,
                settings.PlayerStart,
                view.Direction,
                hits,
                distance);

            var colliders = new List<Transform>(size);
            for (int i = 0; i < size; i++)
            {
                colliders.Add(hits[i].collider.transform);
            }

            _obstacles = colliders
                .OrderBy(a => GetDistanceAlongTrack(a.position))
                .ToArray();
        }

        public float GetDistanceAlongTrack(Vector3 worldPosition)
        {
            Vector3 flat = worldPosition - view.PlayerOrigin;
            flat.y = 0f;
            return Vector3.Dot(flat, view.Direction);
        }

        public void ExplodeAt(float explosionTrackDistance, float explosionRadius)
        {
            while (_obstacleIndex < _obstacles.Length - 1)
            {
                Transform obstacle = _obstacles[_obstacleIndex];
                float obstacleTrackDistance = GetDistanceAlongTrack(obstacle.position);
                float distance = Mathf.Abs(obstacleTrackDistance - explosionTrackDistance);

                if (distance <= explosionRadius || distance <= Mathf.Epsilon)
                {
                    obstacle.gameObject.SetActive(false);
                    _obstacleIndex++;
                }
                else
                {
                    break;
                }
            }
        }

        public void PlayExplosionAt(float trackDistance, float radius)
        {
            if (view == null || settings == null)
                return;

            view.PlayExplosion(trackDistance, radius, settings.ExplosionDuration);
        }

        public float GetNextStopPosition()
        {
            if (HasCurrentObstacle)
            {
                float obstacleDistance = GetDistanceAlongTrack(CurrentObstacle.position);
                float stopPosition = obstacleDistance - settings.StopDistance;
                return Mathf.Max(_model.PlayerPosition, stopPosition);
            }

            Vector3 doorPosition = _obstacles is {Length: > 0}
                ? _obstacles[^1].position
                : view.DoorPosition;

            float doorDistance = GetDistanceAlongTrack(doorPosition);
            return doorDistance + settings.DoorPassDistance;
        }
    }
}
