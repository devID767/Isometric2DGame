using UnityEngine;

namespace Isometric2DGame.Environment
{
    public class AreaPatrolRoute : PatrolRoute
    {
        [SerializeField] private int _tries = 10;
        [SerializeField] private float _minStep = 1.5f;
        [SerializeField] private float _radius = 5f;
        [SerializeField] private float _clearance = 0.2f;
        [SerializeField] private LayerMask _obstacleMask;

        private Vector2 Center => transform.position;

        private Vector2 _lastPoint;

        public override bool TryGetNextPoint(Vector2 from, out Vector2 point)
        {
            point = from;
            for (int i = 0; i < _tries; i++)
            {

                point = Center + Random.insideUnitCircle * _radius;
                var dist = Vector2.Distance(point, from);

                if (dist < _minStep)
                    continue;

                if (Physics2D.OverlapCircle(point, _clearance, _obstacleMask))
                    continue;

                var dir = point - from;
                if (Physics2D.CircleCast(from, _clearance, dir, dist, _obstacleMask))
                    continue;

                _lastPoint = point;
                return true;
            }
            return false;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Center, _radius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_lastPoint == Vector2.zero ? Center : _lastPoint, _clearance);
        }
    }
}
