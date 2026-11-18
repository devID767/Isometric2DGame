using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    [CreateAssetMenu(menuName = "Data/Enemy/EnemyConfig", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [field: Min(0)]
        [field: SerializeField] public float DetectRadius { get; private set; } = 4f;
        [field: Min(0)]
        [field: SerializeField] public float LoseRadius { get; private set; } = 6f;
        
        [field: Min(0)]
        [field: SerializeField] public float AttackDamage { get; private set; } = 1;
        [field: Min(0)]
        [field: SerializeField] public float AttackCooldown{ get; private set; } = 0.67f;
        [field: Min(0)]
        [field: SerializeField] public float AttackRange { get; private set; } = 0.8f;
        [field: Min(0)]
        [field: SerializeField] public float AttackExitMargin { get; private set; } = 0.2f;

        [field: Min(0)]
        [field: SerializeField] public float IdleTime { get; private set; } = 1f;
        [field: SerializeField] public float SearchHoldTime { get; private set; } = 3f;

        [field: SerializeField] public LayerMask PlayerMask { get; private set; }
        [field: SerializeField] public LayerMask ObstackeMask { get; private set; }

        void OnValidate()
        {
            if (LoseRadius < DetectRadius)
                LoseRadius = DetectRadius + 0.1f;

            if(AttackExitMargin > AttackRange)
                AttackExitMargin = AttackRange;
        }
    }
}