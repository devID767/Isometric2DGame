using Isometric2DGame.Animations;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.Movement;
using Isometric2DGame.StateMachines;
using UnityEngine;

namespace Isometric2DGame.EnemyCharacter
{
    public class AttackEnemyState : ITickableState
    {
        private readonly Timer _attackCooldown;
        private readonly float _damage;
        private readonly CharacterAnimator _animator;
        private readonly IPositionHolder _self;
        private readonly PlayerSensor _sensor;

        public AttackEnemyState(EnemyConfig.AttackSettings settings, IPositionHolder self, PlayerSensor sensor, CharacterAnimator animator)
        {
            _damage = settings.Damage;
            _attackCooldown = new Timer(settings.Cooldown);
            _animator = animator;
            _self = self; 
            _sensor = sensor;
        }

        public void Enter()
        {
            _attackCooldown.SetDone();
        }

        public void Exit()
        {
        }

        public void Tick(float deltaTime)
        {
            _attackCooldown.Tick(deltaTime);

            if (_attackCooldown.IsDone)
            {
                Attack();
                _attackCooldown.Reset();
            }
        }

        private void Attack()
        {
            _animator.Face(_sensor.TargetPosition - _self.Position);
            _animator.PlayAnimation(CharacterAction.Attack, true);
            Debug.Log($"Attacking with damage {_damage}");
        }
    }
}