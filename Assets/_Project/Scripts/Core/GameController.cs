using System.Collections;
using Isometric2DGame.MainCharacter;
using Isometric2DGame.Ui;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Isometric2DGame.Core
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Player _player;

        [SerializeField] private float _gameOverDelay = 1f;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private HealthBarUi _playerHealthBar;

        private void Start()
        {
            _player.Health.Died += OnPlayerDied;
            _playerHealthBar.Bind(_player.Health);
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnPlayerDied()
        {
            StartCoroutine(ShowGameOver());
        }

        private IEnumerator ShowGameOver()
        {
            yield return new WaitForSeconds(_gameOverDelay);
            _gameOverScreen.Show(Restart);
        }

        private void OnDestroy()
        {
            _player.Health.Died -= OnPlayerDied;
        }
    }
}