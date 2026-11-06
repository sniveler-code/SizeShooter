using UnityEngine;

namespace SizeShooter.Game
{
    public sealed class GameModel
    {
        public enum Outcome
        {
            None,
            Win,
            LoseOvercharge,
            LoseDeadEnd
        }

        private readonly LevelSettings _level;
        private float _currentTransfer;

        private float _moveTargetPosition;
        private float _jumpStartPosition;
        private float _jumpEndPosition;
        private float _jumpDuration;
        private float _jumpHeight;
        private float _jumpProgress;

        public float PlayerSize { get; private set; }
        public float PlayerPosition { get; private set; }
        public Outcome Result { get; private set; }
        public float ShotSize { get; private set; }
        public float ShotPosition { get; private set; }
        public float PlayerHeight { get; private set; }

        public GameModel(LevelSettings level)
        {
            _level = level;
            PlayerSize = _level.PlayerStart;
        }

        public void StartMoving(float targetPosition)
        {
            ResetShot();
            _moveTargetPosition = targetPosition;
            StartNextJump();
        }

        private void StartNextJump()
        {
            _jumpStartPosition = PlayerPosition;
            float remainingDistance = _moveTargetPosition - _jumpStartPosition;

            if (remainingDistance <= 0.0001f)
            {
                PlayerPosition = _moveTargetPosition;
                PlayerHeight = 0f;
                _jumpProgress = 1f;
                return;
            }

            float step = Mathf.Min(_level.JumpLength, remainingDistance);
            _jumpEndPosition = _jumpStartPosition + step;

            float ratio = step / _level.JumpLength;
            _jumpDuration = step / _level.PlayerSpeed;
            _jumpHeight = _level.JumpHeight * ratio;
            _jumpProgress = 0f;
        }

        public void BeginCharge()
        {
            _currentTransfer = 0f;
            ShotPosition = PlayerPosition + PlayerSize * 0.5f;
            ShotSize = 0f;
        }

        public void ClearShot() => ResetShot();

        private void ResetShot()
        {
            _currentTransfer = 0f;
            ShotSize = 0f;
        }

        public bool UpdateMoving(float deltaTime)
        {
            if (PlayerPosition >= _moveTargetPosition && _jumpProgress >= 1f)
            {
                PlayerPosition = _moveTargetPosition;
                PlayerHeight = 0f;
                return true;
            }

            _jumpProgress += deltaTime / _jumpDuration;

            if (_jumpProgress >= 1f)
            {
                PlayerPosition = _jumpEndPosition;
                PlayerHeight = 0f;

                if (PlayerPosition >= _moveTargetPosition - 0.0001f)
                {
                    PlayerPosition = _moveTargetPosition;
                    return true;
                }

                StartNextJump();
                return false;
            }

            PlayerPosition = Mathf.Lerp(_jumpStartPosition, _jumpEndPosition, _jumpProgress);
            PlayerHeight = 4f * _jumpHeight * _jumpProgress * (1f - _jumpProgress);

            return false;
        }

        public bool UpdateCharge(float dt)
        {
            float transfer = _level.ChargeRate * dt;
            _currentTransfer += transfer;

            PlayerSize -= transfer;
            ShotSize += transfer * _level.ChargeMultiplier;
            ShotPosition = PlayerPosition + PlayerSize * 0.5f + ShotSize * 0.5f;

            if (!(PlayerSize <= _level.CriticalSize)) return false;

            ShotSize = 0f;
            return true;
        }

        private void CancelCharge()
        {
            PlayerSize += _currentTransfer;
            _currentTransfer = 0f;
            ShotSize = 0f;
        }

        public void UpdateShot(float dt) => ShotPosition += _level.ShotSpeed * dt;

        public bool TryReleaseCharge()
        {
            if (!(_currentTransfer < _level.MinTransfer)) return true;
            CancelCharge();
            return false;
        }

        public float Health => PlayerSize;
        public float HealthMax => _level.PlayerStart;
        public float HealthNormalized => Mathf.Clamp01(PlayerSize / _level.PlayerStart);
        public float CriticalNormalized => Mathf.Clamp01(_level.CriticalSize / _level.PlayerStart);

        public void SetOutcome(Outcome outcome)
        {
            Result = outcome;
        }
    }
}
