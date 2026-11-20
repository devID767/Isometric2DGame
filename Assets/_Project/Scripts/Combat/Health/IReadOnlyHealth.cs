using System;

namespace Isometric2DGame.Combat
{
    public interface IReadOnlyHealth
    {
        public HealthData Data { get; }
        public event Action<HealthData> Changed;
        public event Action<DamageInfo> Damaged;
        public event Action Died;
    }
}
