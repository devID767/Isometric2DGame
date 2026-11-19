using UnityEngine;

namespace Isometric2DGame.Movement
{
    public class CharacterMotor
    {
        public Vector2 Velocity => _rb.linearVelocity;

        private Vector2 _direction;

        private float _speedScale = 1f;

        private readonly Rigidbody2D _rb;
        private readonly CharacterMoveConfig _config;

        public CharacterMotor(Rigidbody2D rb, CharacterMoveConfig config)
        {
            _rb = rb;
            _config = config;
        }

        public void SetDirection(Vector2 direction, float speedScale = 1f)
        {
            _direction = direction;
            _speedScale = speedScale;
        }

        public void Reset()
        {
            _speedScale = 1f;
            _direction = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
        }

        public void FixedTick()
        {
            var target = _direction * _config.MaxSpeed * _speedScale;
            _rb.linearVelocity = _config.InstantAcceleration
                ? target
                : Vector2.MoveTowards(_rb.linearVelocity, target, _config.Acceleration * Time.fixedDeltaTime);
        }
    }
}
