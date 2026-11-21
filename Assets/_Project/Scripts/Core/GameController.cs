using System.Collections;
using System.Collections.Generic;
using Isometric2DGame.EnemyCharacter.Spawner;
using Isometric2DGame.MainCharacter;
using Isometric2DGame.Ui;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Isometric2DGame.Core
{
    public enum GameState
    {
        Play,
        GameOver
    }
    
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private HealthBarUi _playerHealthBar;

        [Space]
        [SerializeField] private List<EnemySpawnPoint> _enemySpawnerPoints;
        [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;

        [Space]
        [SerializeField] private float _gameOverDelay = 1f;
        [SerializeField] private GameOverScreen _gameOverScreen;

        private GameState _gameState;

        private EnemySpawner _enemySpawner;

        void Awake()
        {
            _enemySpawner = new EnemySpawner(_player, _enemySpawnerPoints, _enemySpawnerConfig);

            _gameState = GameState.Play;
        }

        private void Start()
        {
            _player.Health.Died += OnPlayerDied;
            _playerHealthBar.Bind(_player.Health);
        }

        private void Update()
        {
            if(_gameState == GameState.Play)
                _enemySpawner.Tick(Time.deltaTime);
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
            _gameState = GameState.GameOver;
            yield return new WaitForSeconds(_gameOverDelay);
            _gameOverScreen.Show(Restart);
        }

        private void OnDestroy()
        {
            _player.Health.Died -= OnPlayerDied;
        }
    }
}