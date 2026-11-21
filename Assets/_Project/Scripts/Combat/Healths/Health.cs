using System;
using UnityEngine;

namespace Isometric2DGame.Combat.Healths
{
    public class Health : IReadOnlyHealth
    {
        private HealthData _data;
        public HealthData Data => _data;

        public event Action<HealthData> Changed;
        public event Action<DamageInfo> Damaged;
        public event Action Died;

        public Health(int max)
        {
            _data = new HealthData(max, max);
        }

        public void TakeDamage(DamageInfo info)
        {
            var value = info.Amount;
            if (!_data.IsAlive || value <= 0)
                return;

            _data = new HealthData(Mathf.Max(0, _data.Current - value), _data.Max);
            Damaged?.Invoke(info);
            Changed?.Invoke(_data);

            if (_data.Current == 0)
                Died?.Invoke();
        }

        public void Reset()
        {
            _data = new HealthData(_data.Max, _data.Max);
            Changed?.Invoke(_data);
        }
    }
}
