using UnityEngine;

namespace Isometric2DGame.EnemyCharacter.Spawner
{
    [CreateAssetMenu(menuName = "Data/Enemy/EnemySpawnerConfig", fileName = "EnemySpawnerConfig")]
    public class EnemySpawnerConfig : ScriptableObject
    {
        [field: SerializeField] public int InitialCount { get; private set; } = 4;
        [field: SerializeField] public int MaxAlive { get; private set; } = 10;
        [field: SerializeField] public float Interval { get; private set; } = 4f;
        [field: SerializeField] public float MinDistanceToPlayer { get; private set; } = 5f;

        [field: Space]
        [field: SerializeField] public Enemy Prefab { get; private set; }
    }
}