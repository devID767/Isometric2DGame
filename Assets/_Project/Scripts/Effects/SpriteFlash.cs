using Isometric2DGame.Infrastructure;
using UnityEngine;

namespace Isometric2DGame.Effects
{
    public class SpriteFlash
    {
        private readonly DamageFlash.FlashSettings _settings;
        private readonly SpriteRenderer _renderer;

        private readonly CooldownTimer _durationTimer;
        private readonly CooldownTimer _blinkTimer;

        public SpriteFlash(DamageFlash.FlashSettings settings, SpriteRenderer renderer)
        {
            _settings = settings;
            _renderer = renderer;

            _durationTimer = new CooldownTimer(_settings.Duration);
            _blinkTimer = new CooldownTimer(_settings.Duration / _settings.BlinkCount);
            _durationTimer.SetDone();
        }

        public void Play()
        {
            SetColor(_settings.Color);
            _blinkTimer.Reset();
            _durationTimer.Reset();
        }

        public void Tick(float deltaTime)
        {
            if (_durationTimer.IsDone)
                return;

            _durationTimer.Tick(deltaTime);
            _blinkTimer.Tick(deltaTime);

            if (_blinkTimer.IsDone)
            {
                SetColor(_renderer.color == _settings.Color ? Color.white : _settings.Color);
                _blinkTimer.Reset();
            }

            if (_durationTimer.IsDone)
                SetColor(Color.white);
        }

        public void Reset()
        {
            SetColor(Color.white);
            _durationTimer.SetDone();
        }

        private void SetColor(Color color)
        {
            _renderer.color = color;
        }
    }
}
