using Isometric2DGame.Infrastructure;
using UnityEngine;

namespace Isometric2DGame.Combat.Projectiles
{
    public class ProjectileCaster
    {
        private readonly Transform _originPoint;
        private readonly ProjectileCasterSettings _settings;

        private readonly CooldownTimer _cooldown;
        private readonly TargetQuery _targetQuery;
        private readonly ComponentPool<Projectile> _projectilesPool;

        private readonly LayerMask _hitMask;

        public ProjectileCaster(Transform originPoint, ProjectileCasterSettings settings)
        {
            _originPoint = originPoint;
            _settings = settings;

            _targetQuery = new TargetQuery(_settings.Enemy);

            _cooldown = new CooldownTimer(settings.Interval);
            _projectilesPool = new ComponentPool<Projectile>(_settings.Prefab);

            _hitMask = _settings.Enemy | _settings.Obstacles;
        }

        public void Tick(float deltaTime)
        {
            _cooldown.Tick(deltaTime);

            if (_cooldown.IsDone && GetClosestTargetPosition(out Vector2 targetPosition))
            {
                Shoot(_originPoint.position, targetPosition);
                _cooldown.Reset();
            }
        }

        private void Shoot(Vector2 from, Vector2 to)
        {
            var direction = (to - from).normalized;
            _projectilesPool.Get().Launch(from, direction, _settings.Damage, _settings.Speed, _settings.LifeTime, _hitMask, _projectilesPool.Release);
        }

        private bool GetClosestTargetPosition(out Vector2 targetPosition)
        {
            var originPosition = _originPoint.position;
            
            targetPosition = default;
            var targets = _targetQuery.Overlap(originPosition, _settings.Range);

            var found = false;

            var minDistance = float.MaxValue;
            foreach (var target in targets)
            {
                if (!HasObstaclesOnLine(originPosition, target.Position))
                {
                    var sqrMagnitude = GetSqrMagnitudeTo(originPosition, target.Position);
                    if (sqrMagnitude < minDistance)
                    {
                        minDistance = sqrMagnitude;
                        targetPosition = target.Position;
                        found = true;
                    }
                }
            }

            return found;
        }

        private float GetSqrMagnitudeTo(Vector2 from, Vector2 to)
        {
            return (to - from).sqrMagnitude;
        }

        private bool HasObstaclesOnLine(Vector2 from, Vector2 to)
        {
            return Physics2D.Linecast(from, to, _settings.Obstacles);
        }
    }
}
