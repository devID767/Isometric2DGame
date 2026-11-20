using UnityEngine;

namespace Isometric2DGame.Movement
{
    public interface IVelocityModifier
    {
        Vector2 Modify(Vector2 velocity, float deltaTime);
    }
}
