using Isometric2DGame.Combat;
using UnityEngine;

namespace Isometric2DGame.Ui
{
    public class HealthBarBinder : MonoBehaviour
    {
        [SerializeField] private Component _owner;
        [SerializeField] private HealthBarUi _bar;

        private void Start()
        {
            _bar.Bind(_owner.GetComponent<IHealthOwner>().Health);
        }

        private void OnValidate()
        {
            if(_owner != null && !_owner.TryGetComponent(out IHealthOwner _))
            {
                Debug.LogWarning($"HealthBarBinder: {_owner.name} must be IHealthOwner");
                _owner = null;
            }
        }
    }
}
