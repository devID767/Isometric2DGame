using UnityEngine;

namespace Isometric2DGame.Movement.Modifiers
{
    public class Knockback : IVelocityModifier
    {
        private readonly float _decay;
        private Vector2 _velocity;

        public Knockback(float decay)
        {
            _decay = decay;
        }

        public void Apply(Vector2 velocity)
        {
            _velocity = velocity;
        }

        public void Reset()
        {
            _velocity = Vector2.zero;
        }

        Vector2 IVelocityModifier.Modify(Vector2 v, float dt)
        {
            var result = v + _velocity;
            _velocity = Vector2.MoveTowards(_velocity, Vector2.zero, _decay * dt);
            return result;
        }
    }
}
