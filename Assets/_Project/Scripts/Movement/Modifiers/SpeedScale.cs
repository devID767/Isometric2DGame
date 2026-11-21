using UnityEngine;

namespace Isometric2DGame.Movement.Modifiers
{
    public class SpeedScale : IVelocityModifier
    {
        public float Value { get; set; } = 1f;

        Vector2 IVelocityModifier.Modify(Vector2 velocity, float deltaTime)
        {
            return velocity * Value;
        }
    }
}
