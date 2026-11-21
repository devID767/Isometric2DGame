using System.Collections.Generic;
using Isometric2DGame.Movement.Modifiers;
using UnityEngine;

namespace Isometric2DGame.Movement
{
    public class CharacterMotor
    {
        private readonly List<IVelocityModifier> _modifiers;
        private Vector2 _moveVelocity;

        public Vector2 Velocity => _moveVelocity;

        private Vector2 _direction;

        private readonly Rigidbody2D _rb;
        private readonly CharacterMoveConfig _config;

        public CharacterMotor(Rigidbody2D rb, CharacterMoveConfig config)
        {
            _rb = rb;
            _config = config;

            _modifiers = new List<IVelocityModifier>();
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        public void AddModifier(IVelocityModifier m)
        {
            _modifiers.Add(m);
        }

        public void Reset()
        {
            _direction = Vector2.zero;
            _moveVelocity = Vector2.zero;
        }

        public void FixedTick()
        {
            var target = _direction * _config.MaxSpeed;
            _moveVelocity = _config.InstantAcceleration
                ? target
                : Vector2.MoveTowards(_moveVelocity, target, _config.Acceleration * Time.fixedDeltaTime);

            var velocity = _moveVelocity;
            foreach (var m in _modifiers)
            {
                velocity = m.Modify(velocity, Time.fixedDeltaTime);
            }
            _rb.linearVelocity = velocity;
        }
    }
}
