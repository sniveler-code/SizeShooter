using UnityEngine;

namespace SizeShooter.Game.View
{
    public sealed class GameView : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private Transform player;
        [SerializeField] private Transform door;
        [SerializeField] private Transform shot;
        [SerializeField] private float cameraFollowSpeed = 10f;
        [SerializeField] private float groundY = 0f;

        private GameModel _model;
        private ExplosionView _explosion;
        private Vector3 _playerOrigin;
        private Vector3 _direction;
        private Collider[] _obstacles;
        private Vector3 _cameraOffset;

        public Vector3 PlayerPosition => player.position;
        public Vector3 PlayerOrigin => _playerOrigin;
        public Vector3 DoorPosition => door.position;
        public Vector3 Direction => _direction;

        private void Awake()
        {
            _playerOrigin = new Vector3(player.position.x, groundY, player.position.z);
            Vector3 toDoor = door.position - _playerOrigin;
            toDoor.y = 0f;
            _direction = toDoor.normalized;
            _cameraOffset = camera.transform.position - _playerOrigin;
        }

        public void Init(GameModel model) => _model = model;

        private void OnDestroy() => _explosion?.Dispose();

        public void PlayExplosion(float trackDistance, float radius, float duration)
        {
            Vector3 groundPosition = _playerOrigin + _direction * trackDistance;
            Vector3 center = groundPosition + Vector3.up * Mathf.Max(0.3f, radius);
            _explosion ??= new ExplosionView(transform);
            _explosion.Play(center, _direction, Mathf.Max(0.55f, radius * 2f), duration);
        }

        public void Render(float deltaTime)
        {
            Vector3 playerGroundPos = _playerOrigin + _direction * _model.PlayerPosition;
            float playerRadius = _model.PlayerSize * 0.5f;

            player.localScale = new Vector3(_model.PlayerSize, _model.PlayerSize, _model.PlayerSize);
            player.position = playerGroundPos + Vector3.up * (playerRadius + _model.PlayerHeight);

            Vector3 shotGroundPos = _playerOrigin + _direction * _model.ShotPosition;
            float shotRadius = _model.ShotSize * 0.5f;
            shot.localScale = new Vector3(_model.ShotSize, _model.ShotSize, _model.ShotSize);
            shot.position = shotGroundPos + Vector3.up * shotRadius;

            _explosion?.Render(deltaTime);

            Vector3 targetPosition = playerGroundPos + _cameraOffset;
            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                targetPosition,
                cameraFollowSpeed * deltaTime);
        }
    }
}
