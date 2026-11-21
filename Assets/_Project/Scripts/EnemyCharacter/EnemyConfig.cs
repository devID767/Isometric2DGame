using System;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    [CreateAssetMenu(menuName = "Data/Enemy/EnemyConfig", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [Serializable]
        public class AttackSettings
        {
            [field: Min(0)]
            [field: SerializeField] public int Damage { get; private set; } = 1;
            [field: Min(0)]
            [field: SerializeField] public float Range { get; private set; } = 0.4f;
            [field: Min(0)]
            [field: SerializeField] public float ExitRange { get; private set; } = 0.6f;

            [field: SerializeField] public Combat.Swing.SwingSettings Swing { get; private set; } = new ();

            public void Validate()
            {
                if (ExitRange < Range)
                    ExitRange = Range + 0.1f;

                Swing.Validate();
            }
        }

        [Serializable]
        public class DetectionSettings
        {
            [field: Min(0)]
            [field: SerializeField] public float DetectRadius { get; private set; } = 4f;
            [field: Min(0)]
            [field: SerializeField] public float LoseRadius { get; private set; } = 6f;
            [field: SerializeField] public LayerMask PlayerMask { get; private set; }
            [field: SerializeField] public LayerMask ObstackeMask { get; private set; }

            public void Validate()
            {
                if (LoseRadius < DetectRadius)
                    LoseRadius = DetectRadius + 0.1f;
            }
        }

        [Serializable]
        public class PatrolSettings
        {
            [field: Min(0)]
            [field: SerializeField] public float PointTimeout { get; private set; } = 15f;
        }

        [Serializable]
        public class SearchSettings
        {
            [field: Min(0)]
            [field: SerializeField] public float HoldTime { get; private set; } = 1f;
            [field: Min(0)]
            [field: SerializeField] public float Timeout { get; private set; } = 15f;
            [field: Min(1)]
            [field: SerializeField] public int LookCount { get; private set; } = 6;
        }

        [Serializable]
        public class ChaseSettings
        {
            [field: Min(0)]
            [field: SerializeField] public float SpeedMultiplier { get; private set; } = 1.25f;
        }

        [Serializable]
        public class IdleSettings
        {
            [field: Min(0)]
            [field: SerializeField] public float HoldTime { get; private set; } = 1f;
        }

        [Serializable]
        public class HealthSettings
        {
            [field: Min(0)]
            [field: SerializeField] public int Max { get; private set; } = 5;
        }

        [Serializable]
        public class HurtSettings
        {
            [field: Min(0.01f)]
            [field: SerializeField] public float Duration { get; private set; } = 0.3f;
            [field: Min(0)]
            [field: SerializeField] public float PushSpeed { get; private set; } = 5f;
        }

        [Serializable]
        public class DeadSettings
        {
            [field: Min(0.01f)]
            [field: SerializeField] public float CorpseTime { get; private set; } = 3f;
        }

        [field: SerializeField] public DetectionSettings Detection { get; private set; } = new();
        [field: SerializeField] public IdleSettings Idle { get; private set; } = new();
        [field: SerializeField] public PatrolSettings Patrol { get; private set; } = new();
        [field: SerializeField] public ChaseSettings Chase { get; private set; } = new();
        [field: SerializeField] public AttackSettings Attack { get; private set; } = new();
        [field: SerializeField] public SearchSettings Search { get; private set; } = new();
        [field: SerializeField] public HurtSettings Hurt { get; private set; } = new();
        [field: SerializeField] public DeadSettings Dead { get; private set; } = new();

        [field: Space]
        [field: SerializeField] public HealthSettings Health { get; private set; } = new();

        private void OnValidate()
        {
            Attack.Validate();
            Detection.Validate();
        }
    }
}