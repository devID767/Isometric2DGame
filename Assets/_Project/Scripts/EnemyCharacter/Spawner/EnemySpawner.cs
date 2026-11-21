using System.Collections.Generic;
using Isometric2DGame.Infrastructure;

namespace Isometric2DGame.EnemyCharacter.Spawner
{
    public class EnemySpawner
    {
        private readonly IPositionHolder _playerPosition;
        private readonly List<EnemySpawnPoint> _spawnPoints;
        private readonly EnemySpawnerConfig _config;

        private readonly ComponentPool<Enemy> _pool;
        private readonly CooldownTimer _cooldown;

        private readonly List<EnemySpawnPoint> _candidates;

        public EnemySpawner(IPositionHolder playerPosition, List<EnemySpawnPoint> spawnPoints, EnemySpawnerConfig config)
        {
            _playerPosition = playerPosition;
            _spawnPoints = spawnPoints;
            _config = config;

            _pool = new ComponentPool<Enemy>(_config.Prefab);
            _cooldown = new CooldownTimer(_config.Interval);

            _candidates = new List<EnemySpawnPoint>();

            SpawnStarts();
        }

        public void Tick(float deltaTime)
        {
            if (_pool.CountActive >= _config.MaxAlive)
                return;

            _cooldown.Tick(deltaTime);
            if (_cooldown.IsDone && TryGetRandomPoint(out EnemySpawnPoint spawnPoint))
            {
                Spawn(spawnPoint);
                _cooldown.Reset();
            }
        }

        private void SpawnStarts()
        {
            for (int i = 0; i < _config.InitialCount; i++)
            {
                TryGetRandomPoint(out EnemySpawnPoint point);
                Spawn(point);
            }
        }

        private bool TryGetRandomPoint(out EnemySpawnPoint result)
        {
            result = default;
            
            _candidates.Clear();
            foreach (var point in _spawnPoints)
                if ((point.Position - _playerPosition.Position).sqrMagnitude > _config.MinDistanceToPlayer * _config.MinDistanceToPlayer)
                    _candidates.Add(point);

            if (_candidates.Count == 0)
                 return false;

            result = _candidates[UnityEngine.Random.Range(0, _candidates.Count)];
            return true;
        }

        private bool TryGetClosestPoint(out EnemySpawnPoint result)
        {
            var minAllowedSqrMagnitude = float.MaxValue;
            var found = false;
            result = default;

            foreach (var spawnPoint in _spawnPoints)
            {
                var sqrMagnitude = (spawnPoint.Position - _playerPosition.Position).sqrMagnitude;
                if (sqrMagnitude > _config.MinDistanceToPlayer * _config.MinDistanceToPlayer
                    && sqrMagnitude < minAllowedSqrMagnitude)
                {
                    minAllowedSqrMagnitude = sqrMagnitude;
                    result = spawnPoint;
                    found = true;
                }
            }

            return found;
        }

        private void Spawn(EnemySpawnPoint spawnPoint)
        {
            var enemy = _pool.Get(e =>
            {
                e.transform.position = spawnPoint.Position;
                e.SetupRoute(spawnPoint.PatrolRoute);
            });

            enemy.CorpseExpired += OnCorpseExpired;
        }

        private void OnCorpseExpired(Enemy enemy)
        {
            enemy.CorpseExpired -= OnCorpseExpired;
            _pool.Release(enemy);
        }
    }
}