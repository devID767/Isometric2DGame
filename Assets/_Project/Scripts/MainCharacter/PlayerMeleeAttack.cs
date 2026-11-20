using System.Collections.Generic;
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

        private readonly List<Collider2D> _results;
        private readonly ContactFilter2D _filter;
        private readonly HashSet<IDamageable> _damageableHashes;

        private readonly Swing _swing;

        public bool Swinging => _swing.Swinging;

        private Vector2 _facing;

        public PlayerMeleeAttack(IPositionHolder self, PlayerCombatConfig.AttackSettings settings, SpeedScale attackSlow)
        {
            _self = self;
            _settings = settings;
            _attackSlow = attackSlow;

            _swing = new Swing(_settings.Swing);

            _results = new List<Collider2D>();
            _filter = new ContactFilter2D();
            _filter.SetLayerMask(_settings.Enemy);
            _filter.useTriggers = false;

            _damageableHashes = new HashSet<IDamageable>();
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
            _damageableHashes.Clear();

            var center = _self.Position + _facing * _settings.Offset;
            var count = Physics2D.OverlapCircle(center, _settings.Radius, _filter, _results);

            for (int i = 0; i < count; i++)
            {
                var targetRb = _results[i].attachedRigidbody;
                if (targetRb != null && targetRb.TryGetComponent(out IDamageable damageable))
                {
                    if (!_damageableHashes.Contains(damageable))
                    {
                        damageable.TakeDamage(new DamageInfo(_settings.Damage, _self.Position));
                        _damageableHashes.Add(damageable);
                    }
                }
            }
        }
    }
}
