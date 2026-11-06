using UnityEngine;

namespace SizeShooter.Game
{
    [CreateAssetMenu(menuName = "SizeShooter/Level Settings", fileName = "Level_00")]
    public sealed class LevelSettings : ScriptableObject
    {
        [SerializeField] private float criticalSize = 0.15f;
        [SerializeField] private float chargeRate = 0.3f;
        [SerializeField] private float shotSpeed = 8f;
        [SerializeField] private float playerStart = 2.4f;
        [SerializeField] private float stopDistance = 0.75f;
        [SerializeField] private float playerSpeed = 2.1f;
        [SerializeField] private float chargeMultiplier = 1.5f;
        [SerializeField] private float minTransfer = 0.1f;

        [Header("Frog Jump")] [SerializeField] private float jumpLength = 1.2f;
        [SerializeField] private float jumpHeight = 0.6f;
        [SerializeField] private float doorPassDistance = 1.5f;

        [Header("Explosion")] [SerializeField] private float explosionDuration = 1f;

        public float StopDistance => stopDistance;
        public float ChargeRate => chargeRate;
        public float CriticalSize => criticalSize;
        public float ShotSpeed => shotSpeed;
        public float PlayerSpeed => playerSpeed;
        public float PlayerStart => playerStart;
        public float ChargeMultiplier => chargeMultiplier;
        public float MinTransfer => minTransfer;
        public float JumpLength => jumpLength;
        public float JumpHeight => jumpHeight;
        public float DoorPassDistance => doorPassDistance;
        public float ExplosionDuration => explosionDuration > 0f ? explosionDuration : 1f;
    }
}
