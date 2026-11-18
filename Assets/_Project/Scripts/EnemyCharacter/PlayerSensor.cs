using Isometric2DGame.Movement;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    public class PlayerSensor
    {
        private readonly IPositionHolder _positionHolder;
        private readonly EnemyConfig _enemyConfig;

        private float _detectRadius => _enemyConfig.DetectRadius;
        private float _loseRadius => _enemyConfig.LoseRadius;
        private LayerMask _playerMask => _enemyConfig.PlayerMask;
        private LayerMask _obstacleMask => _enemyConfig.ObstackeMask;

        public bool HasTarget { get; private set; }
        public Vector2 TargetPosition { get; private set; }
        public float Distance { get; private set; }
        public bool HasLineOfSight { get; private set; }

        public bool CanSee(float radius) => HasTarget && HasLineOfSight && Distance <= radius;

        public PlayerSensor(IPositionHolder positionHolder, EnemyConfig enemyConfig)
        {
            _positionHolder = positionHolder;
            _enemyConfig = enemyConfig;
        }

        public void Refresh()
        {
            HasTarget = IsPlayerNearby(_loseRadius, out Vector2 playerPosition);
            TargetPosition = playerPosition;
            if (!HasTarget)
            {
                Distance = float.PositiveInfinity;
                HasLineOfSight = false;
            }
            else
            {
                Distance = Vector2.Distance(_positionHolder.Position, playerPosition);
                HasLineOfSight = !HasObstaclesOnLine(playerPosition);
            }
        }

        private bool IsPlayerNearby(float radius, out Vector2 position)
        {
            var playerCollider = Physics2D.OverlapCircle(_positionHolder.Position, radius, _playerMask);
            if (playerCollider != null)
            {
                position = playerCollider.attachedRigidbody ? 
                    playerCollider.attachedRigidbody.position : playerCollider.transform.position;
                return true;
            }

            position = Vector2.zero;
            return false;
        }

        private bool HasObstaclesOnLine(Vector2 playerPosition)
        {
            var hit = Physics2D.Linecast(_positionHolder.Position, playerPosition, _obstacleMask);
            return hit;
        }
    }
}