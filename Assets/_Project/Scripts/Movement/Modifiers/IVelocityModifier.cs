using UnityEngine;

namespace Isometric2DGame.Movement.Modifiers
{
    public interface IVelocityModifier
    {
        Vector2 Modify(Vector2 velocity, float deltaTime);
    }
}
