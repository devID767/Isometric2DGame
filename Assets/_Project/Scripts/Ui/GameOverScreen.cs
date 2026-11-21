using System;
using UnityEngine;
using UnityEngine.UI;

namespace Isometric2DGame.Ui
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _restartButton;

        private Action _onRestart;

        private void Awake()
        {
            _panel.SetActive(false);
            _restartButton.onClick.AddListener(OnRestartClicked);
        }

        public void Show(Action onRestart)
        {
            _onRestart = onRestart;
            
            _panel.SetActive(true);
            _restartButton.Select();
        }

        private void OnRestartClicked()
        {
            _onRestart?.Invoke();
        }

        private void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(OnRestartClicked);
        }
    }
}