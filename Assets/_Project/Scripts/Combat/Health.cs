using System;
using UnityEngine;

namespace Isometric2DGame.Combat
{
    public class Health
    {
        public int Max { get; private set; }
        public int Current { get; private set; }

        public bool IsAlive => Current > 0;

        public event Action Changed;
        public event Action Damaged;
        public event Action Died;

        public Health(int max)
        {
            Max = max;
            Current = max;
        }

        public void TakeDamage(int value)
        {
            if (!IsAlive || value <= 0)
                return;

            Current = Mathf.Max(0, Current - value);
            Damaged?.Invoke();
            Changed?.Invoke();

            if (Current == 0)
                Died?.Invoke();
        }

        public void Reset()
        {
            Current = Max;
            Changed?.Invoke();
        }
    }
}
