using System;
using UnityEngine;

namespace Isometric2DGame.Combat.Projectiles
{    
    [Serializable]
    public class ProjectileCasterSettings
    {
        [field: Min(1)]
        [field: SerializeField] public int Damage { get; private set; } = 1;

        [field: Min(0)]
        [field: SerializeField] public float Interval { get; private set; } = 1f;
        [field: Min(0)]
        [field: SerializeField] public float Range { get; private set; } = 5f;
        [field: Min(0)]
        [field: SerializeField] public float Speed { get; private set; } = 1f;
        [field: Min(0)]
        [field: SerializeField] public float LifeTime { get; private set; } = 3f;

        [field: SerializeField] public LayerMask Enemy { get; private set; }
        [field: SerializeField] public LayerMask Obstacles { get; private set; }
        [field: SerializeField] public Projectile Prefab { get; private set; }
    }
}
