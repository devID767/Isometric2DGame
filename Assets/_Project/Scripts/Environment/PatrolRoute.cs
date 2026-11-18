using UnityEngine;

namespace Isometric2DGame.Environment
{
    public abstract class PatrolRoute : MonoBehaviour
    {
        public abstract bool TryGetNextPoint(Vector2 from, out Vector2 point);
    }
}
