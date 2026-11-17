using UnityEngine;

namespace Isometric2DGame.Movement
{
    [CreateAssetMenu(menuName = "Data/Character/CharacterMoveConfig", fileName = nameof(CharacterMoveConfig))]
    public class CharacterMoveConfig : ScriptableObject
    {
        [field: Min(0)]
        [field: SerializeField] public float MaxSpeed { get; private set; } = 5f;
        [field: SerializeField] public bool InstantAcceleration { get; private set; } = true;
        [field: Min(0)]
        [field: SerializeField] public float Acceleration { get; private set; } = 15f;
    }
}
