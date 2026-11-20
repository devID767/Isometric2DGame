using System;
using Isometric2DGame.Combat;
using UnityEngine;

namespace Isometric2DGame.Effects
{
    public class DamageFlash : MonoBehaviour
    {
        [Serializable]
        public class FlashSettings
        {
            [field: SerializeField] public Color Color { get; private set; } = Color.red;
            [field: Min(1)]
            [field: SerializeField] public int BlinkCount { get; private set; } = 8;
            [field: Min(0)]
            [field: SerializeField] public float Duration { get; private set; } = 0.5f;
        }

        [SerializeField] private FlashSettings _settings;
        [SerializeField] private Component _owner;
        [SerializeField] private SpriteRenderer _renderer;

        private SpriteFlash _spriteFlash;

        private IReadOnlyHealth _health;

        private void Awake()
        {
            _spriteFlash = new SpriteFlash(_settings, _renderer);
        }

        private void Start()
        {
            _health = _owner.GetComponent<IHealthOwner>().Health;

            _health.Damaged += OnDamaged;
            _health.Died += OnDied;
        }

        private void Update()
        {
            _spriteFlash.Tick(Time.deltaTime);
        }

        private void OnDamaged(DamageInfo info)
        {
            _spriteFlash.Play();
        }

        private void OnDied()
        {
            // _spriteFlash.Reset();
        }

        private void OnDisable()
        {
            _spriteFlash.Reset();
        }

        private void OnDestroy()
        {
            if(_health == null)
                return;
            
            _health.Damaged -= OnDamaged;
            _health.Died -= OnDied;
        }

        private void OnValidate()
        {
            if (_owner != null && !_owner.TryGetComponent(out IHealthOwner _))
            {
                Debug.LogWarning($"DamageFlash: {_owner.name} must be IHealthOwner");
                _owner = null;
            }
        }
    }
}
