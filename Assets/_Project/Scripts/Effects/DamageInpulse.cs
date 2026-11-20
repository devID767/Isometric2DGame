using Isometric2DGame.Combat;
using Unity.Cinemachine;
using UnityEngine;

namespace Isometric2DGame.Effects
{
    public class DamageInpulse : MonoBehaviour
    {
        [SerializeField] private Component _owner;
        [SerializeField] private CinemachineImpulseSource _source;
        [SerializeField] private float _force = 0.15f;

        private IReadOnlyHealth _health;

        private void Start()
        {
            _health = _owner.GetComponent<IHealthOwner>().Health;
            _health.Damaged += OnDamaged;
        }

        private void OnDamaged(DamageInfo info)
        {
            var impulse = info.DirectionTo(_owner.transform.position) * _force * info.Amount;
            _source.GenerateImpulseWithVelocity(impulse);
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.Damaged -= OnDamaged;
        }

        private void OnValidate()
        {
            if (_owner != null && !_owner.TryGetComponent(out IHealthOwner _))
            {
                Debug.LogWarning($"DamageInpulse: {_owner.name} must be IHealthOwner");
                _owner = null;
            }
        }
    }
}
