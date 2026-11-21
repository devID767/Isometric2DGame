using Isometric2DGame.Combat;
using Isometric2DGame.Infrastructure;
using Isometric2DGame.Movement;
using UnityEngine;

namespace Isometric2DGame.MainCharacter
{
    public class PlayerMeleeAttack
    {
        private readonly IPositionHolder _self;
        private readonly PlayerCombatConfig.AttackSettings _settings;
        private readonly SpeedScale _attackSlow;

        private readonly TargetQuery _targetQuery;

        private readonly Swing _swing;

        public bool Swinging => _swing.Swinging;

        private Vector2 _facing;

        public PlayerMeleeAttack(IPositionHolder self, PlayerCombatConfig.AttackSettings settings, SpeedScale attackSlow)
        {
            _self = self;
            _settings = settings;
            _attackSlow = attackSlow;

            _swing = new Swing(_settings.Swing);

            _targetQuery = new TargetQuery(_settings.Enemy);
        }

        public bool TryStart(Vector2 facing)
        {
            if (_swing.TryStart())
            {
                _facing = facing;
                return true;
            }
            return false;
        }

        public void Tick(float deltaTime)
        {
            if (_swing.Tick(deltaTime))
            {
                Hit();
            }

            _attackSlow.Value = Swinging ? _settings.MoveSpeedScale : 1f;
        }

        public void Reset()
        {
            _swing.Reset();
            _attackSlow.Value = 1f;
        }

        private void Hit()
        {
            var center = _self.Position + _facing * _settings.Offset;
            var targets = _targetQuery.Overlap(center, _settings.Radius);
            foreach (var target in targets)
            {
                target.TakeDamage(new DamageInfo(_settings.Damage, _self.Position));
            }
        }
    }
}
