using System.Collections.Generic;
using Isometric2DGame.Movement;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    public class PatrolEnemyState : ITickableState
    {
        private const float Threshold = 0.1f;

        private readonly IPositionHolder _posHolder;
        private readonly CharacterMotor _motor;
        private readonly List<Vector2> _points;

        public bool HasPoints => _points != null && _points.Count > 0;

        private int _currentIndex;

        public PatrolEnemyState(IPositionHolder posHolder, CharacterMotor motor, List<Vector2> points)
        {
            _posHolder = posHolder;
            _motor = motor;
            _points = points;
        }

        public void Enter()
        {
            SetNearbyPoint();
        }

        public void Tick(float deltaTime)
        {
            var distance = GetDistance(_currentIndex);
            if (distance < Threshold)
            {
                SetNextPoint();
            }
            var direction = GetDirection();

            _motor.SetDirection(direction);
        }

        private Vector2 GetDirection()
        {
            return (_points[_currentIndex] - _posHolder.Position).normalized;
        }

        private float GetDistance(int index)
        {
            return Vector2.Distance(_points[index], _posHolder.Position);
        }

        private void SetNextPoint()
        {
            _currentIndex++;
            if (_currentIndex > _points.Count - 1)
                _currentIndex = 0;
        }

        private void SetNearbyPoint()
        {
            var closestDistance = float.MaxValue;
            for(int i = 0; i < _points.Count; i++)
            {
                var distance = GetDistance(i);
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    _currentIndex = i;
                }
            }
        }

        public void Exit()
        {
            _motor.SetDirection(Vector2.zero);
        }
    }
}