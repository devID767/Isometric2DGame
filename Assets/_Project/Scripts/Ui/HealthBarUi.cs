using Isometric2DGame.Combat.Healths;
using UnityEngine;
using UnityEngine.UI;

namespace Isometric2DGame.Ui
{
    public class HealthBarUi : MonoBehaviour
    {
        [SerializeField] private bool _hideOnDeath;
        [SerializeField] private Image _image;

        private IReadOnlyHealth _health;

        public void Bind(IReadOnlyHealth health)
        {
            if (_health != null)
                return;

            _health = health;

            _health.Changed += Refresh;
            Refresh(health.Data);
        }

        public void Unbind()
        {
            if (_health == null)
                return;

            _health.Changed -= Refresh;
            _health = null;
        }

        private void Refresh(HealthData data)
        {
            if (_hideOnDeath)
                gameObject.SetActive(data.IsAlive);

            _image.fillAmount = data.Normalized;
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}