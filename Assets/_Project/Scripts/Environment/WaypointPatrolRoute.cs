using System.Collections.Generic;
using UnityEngine;

namespace Isometric2DGame.Environment
{
    public class WaypointPatrolRoute : PatrolRoute
    {
        private const float Threshold = 0.5f;

        [SerializeField] private List<Transform> _points;

        private int _currentIndex;

        public override bool TryGetNextPoint(Vector2 from, out Vector2 point)
        {
            point = from;
            if (_points.Count == 0)
                return false;

            if (GetDistance(_currentIndex, from) < Threshold)
                SetNextPoint();
            else
                SetClosestPoint(from);

            point = _points[_currentIndex].position;

            return true;
        }

        private float GetDistance(int index, Vector2 from)
        {
            return Vector2.Distance(_points[index].position, from);
        }

        private void SetClosestPoint(Vector2 from)
        {
            var minimumDistance = float.MaxValue;
            for (int i = 0; i < _points.Count; i++)
            {
                var distance = GetDistance(i, from);
                if (distance < minimumDistance)
                {
                    _currentIndex = i;
                    minimumDistance = distance;
                }
            }
        }

        private void SetNextPoint()
        {
            _currentIndex++;
            if (_currentIndex > _points.Count - 1)
                _currentIndex = 0;
        }

        void OnDrawGizmosSelected()
        {
            if(_points == null)
                return;
            
            Gizmos.color = Color.green;
            for (int i = 0; i < _points.Count - 1; i++)
            {
                Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
            }
            if (_points.Count > 2)
                Gizmos.DrawLine(_points[0].position, _points[_points.Count - 1].position);
        }
    }
}
