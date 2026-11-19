using Isometric2DGame.Combat;
using Isometric2DGame.Infrastructure;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    public class PlayerSensor
    {
        private readonly IPositionHolder _positionHolder;
        private readonly EnemyConfig.DetectionSettings _config;

        private float _loseRadius => _config.LoseRadius;
        private LayerMask _playerMask => _config.PlayerMask;
        private LayerMask _obstacleMask => _config.ObstackeMask;

        public bool HasTarget { get; private set; }
        public ITarget Target { get; private set; }
        public float Distance { get; private set; }
        public bool HasLineOfSight { get; private set; }
        public Vector2 LastSeenPosition { get; private set; }

        private Collider2D _lastCollider;

        public bool CanSee(float radius) => HasTarget && HasLineOfSight && Distance <= radius;

        public PlayerSensor(IPositionHolder positionHolder, EnemyConfig.DetectionSettings detection)
        {
            _positionHolder = positionHolder;
            _config = detection;
        }

        public void Refresh()
        {
            var found = IsPlayerNearby(_loseRadius, out ITarget target);

            HasTarget = found && target.IsAlive;

            if (HasTarget)
            {
                Distance = Vector2.Distance(_positionHolder.Position, target.Position);
                HasLineOfSight = !HasObstaclesOnLine(target.Position);
                if (HasLineOfSight)
                    LastSeenPosition = target.Position;
            }
            else
            {
                Distance = float.PositiveInfinity;
                HasLineOfSight = false;
            }
        }

        private bool IsPlayerNearby(float radius, out ITarget target)
        {
            var playerCollider = Physics2D.OverlapCircle(_positionHolder.Position, radius, _playerMask);

            if (playerCollider != null)
            {
                if (playerCollider == _lastCollider)
                {
                    target = Target;
                    return true;
                }

                if (playerCollider.attachedRigidbody != null && playerCollider.attachedRigidbody.TryGetComponent(out target))
                {
                    _lastCollider = playerCollider;
                    Target = target;
                    return true;
                }
            }
            target = null;
            return false;
        }

        private bool HasObstaclesOnLine(Vector2 playerPosition)
        {
            var hit = Physics2D.Linecast(_positionHolder.Position, playerPosition, _obstacleMask);
            return hit;
        }
    }
}