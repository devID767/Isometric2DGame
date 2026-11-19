using System;
using Isometric2DGame.Infrastructure;
using UnityEngine;

namespace Isometric2DGame.Combat
{
    public class Swing
    {
        [Serializable]
        public class SwingSettings
        {
            [field: Min(0)]
            [field: SerializeField] public float HitDelay { get; private set; } = 0.15f;
            [field: Min(0)]
            [field: SerializeField] public float Cooldown { get; private set; } = 0.5f;
            [field: Min(0)]
            [field: SerializeField] public float Duration { get; private set; } = 0.45f;

            public void Validate()
            {
                if (Duration > Cooldown)
                    Duration = Cooldown;

                if (HitDelay > Duration)
                    HitDelay = Duration;
            }
        }

        private readonly SwingSettings _settings;

        private readonly Timer _cooldown;
        private readonly Timer _hitDelay;
        private readonly Timer _swingTimer;

        public bool Swinging { get; private set; }

        private bool _hitDone;

        public Swing(SwingSettings settings)
        {
            _settings = settings;

            _cooldown = new Timer(_settings.Cooldown);
            _hitDelay = new Timer(_settings.HitDelay);
            _swingTimer = new Timer(_settings.Duration);
        }

        public bool TryStart()
        {
            if (_cooldown.IsDone && !Swinging)
            {
                _hitDelay.Reset();
                _cooldown.Reset();
                _swingTimer.Reset();
                Swinging = true;
                _hitDone = false;
                return true;
            }
            return false;
        }

        public bool Tick(float deltaTime)
        {
            _cooldown.Tick(deltaTime);

            if (Swinging)
            {
                _hitDelay.Tick(deltaTime);
                _swingTimer.Tick(deltaTime);

                if (_hitDelay.IsDone && !_hitDone)
                {
                    _hitDone = true;
                    return true;
                }

                if (_swingTimer.IsDone)
                    Swinging = false;
            }

            return false;
        }

        public void Reset()
        {
            _cooldown.SetDone();
            _hitDelay.Reset();
            Swinging = false;
        }
    }
}
