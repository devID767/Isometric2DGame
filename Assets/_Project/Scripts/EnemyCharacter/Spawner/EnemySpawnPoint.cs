using Isometric2DGame.EnemyCharacter.Patrol;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter.Spawner
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [field: SerializeField] public PatrolRoute PatrolRoute { get; private set; }

        public Vector2 Position => transform.position;
    }
}