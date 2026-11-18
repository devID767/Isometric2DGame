using UnityEngine;

namespace Isometric2DGame.Movement
{
    public class CharacterMotor
    {
        public Vector2 Velocity => _rb.linearVelocity;

        private Vector2 _direction;

        private readonly Rigidbody2D _rb;
        private readonly CharacterMoveConfig _config;

        public CharacterMotor(Rigidbody2D rb, CharacterMoveConfig config)
        {
            _rb = rb;
            _config = config;
        }

        public void SetMoveInput(Vector2 direction)
        {
            _direction = direction;
        }

        public void FixedTick()
        {
            var target = _direction * _config.MaxSpeed;
            _rb.linearVelocity = _config.InstantAcceleration
                ? target
                : Vector2.MoveTowards(_rb.linearVelocity, target, _config.Acceleration * Time.fixedDeltaTime);
        }
    }
}
