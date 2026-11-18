using Isometric2DGame.Movement;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    public class ChaseEnemyState : ITickableState
    {
        private readonly CharacterMotor _motor;
        private readonly PlayerSensor _sensor;
        private readonly IPositionHolder _positionHolder;

        public ChaseEnemyState(IPositionHolder positionHolder, CharacterMotor motor, PlayerSensor sensor)
        {
            _positionHolder = positionHolder;
            _motor = motor;
            _sensor = sensor;
        }

        public void Enter()
        {
        }

        public void Tick(float deltaTime)
        {
            _motor.SetDirection(GetDirection());
        }

        private Vector2 GetDirection()
        {
            return (_sensor.TargetPosition - _positionHolder.Position).normalized;
        }

        public void Exit()
        {
            _motor.SetDirection(Vector2.zero);
        }
    }
}