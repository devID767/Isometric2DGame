using UnityEngine;

namespace Isometric2DGame.MainCharacter
{
    [CreateAssetMenu(menuName = "Data/Player/PlayerCombatConfig", fileName = "PlayerCombatConfig")]
    public class PlayerCombatConfig : ScriptableObject
    {
        [field: Min(1)]
        [field: SerializeField] public int MaxHP {get; private set;} = 10;
    }
}
