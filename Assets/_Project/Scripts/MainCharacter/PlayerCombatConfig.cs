using System;
using Isometric2DGame.Combat.Projectiles;
using UnityEngine;

namespace Isometric2DGame.MainCharacter
{
    [CreateAssetMenu(menuName = "Data/Player/PlayerCombatConfig", fileName = "PlayerCombatConfig")]
    public class PlayerCombatConfig : ScriptableObject
    {
        [Serializable]
        public class AttackSettings
        {
            [field: Min(1)]
            [field: SerializeField] public int Damage { get; private set; } = 1;

            [field: Range(0, 1)]
            [field: SerializeField] public float MoveSpeedScale { get; private set; } = 0.5f;

            [field: Min(0)]
            [field: SerializeField] public float Offset { get; private set; } = 0.35f;
            [field: Min(0)]
            [field: SerializeField] public float Radius { get; private set; } = 0.35f;

            [field: SerializeField] public Combat.Swing.SwingSettings Swing { get; private set; } = new();

            [field: SerializeField] public LayerMask Enemy { get; private set; }

            public void Validate()
            {
                Swing.Validate();
            }
        }

        [Serializable]
        public class HurtSettings
        {
            [field: Min(0.01f)]
            [field: SerializeField] public float Duration { get; private set; } = 0.25f;
            [field: Min(0)]
            [field: SerializeField] public float InvulnerableDuration { get; private set; } = 0.5f;
            [field: Min(0)]
            [field: SerializeField] public float PushSpeed { get; private set; } = 5f;
        }

        [field: Min(1)]
        [field: SerializeField] public int MaxHP { get; private set; } = 10;
        [field: SerializeField] public AttackSettings Attack { get; private set; } = new();
        [field: SerializeField] public ProjectileCasterSettings Projectile { get; private set; } = new();
        [field: SerializeField] public HurtSettings Hurt { get; private set; } = new();

        void OnValidate()
        {
            Attack.Validate();
        }
    }
}
