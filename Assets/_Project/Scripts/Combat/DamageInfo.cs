using UnityEngine;

namespace Isometric2DGame.Combat
{
    public readonly struct DamageInfo
    {
        public int Amount { get; }
        public Vector2 Source { get; }

        public DamageInfo(int amount, Vector2 source)
        {
            Amount = amount;
            Source = source;
        }

        public Vector2 DirectionTo(Vector2 position) => (position - Source).normalized;
    }
}
