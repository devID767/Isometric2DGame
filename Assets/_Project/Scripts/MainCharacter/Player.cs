using Isometric2DGame.Animations;
using Isometric2DGame.Combat;
using Isometric2DGame.Combat.Healths;
using Isometric2DGame.Combat.Projectiles;
using Isometric2DGame.Movement;
using Isometric2DGame.Movement.Modifiers;
using UnityEngine;

namespace Isometric2DGame.MainCharacter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, ITarget
    {
        [SerializeField] private Transform _shootPoint;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterMoveConfig _moveConfig;
        [SerializeField] private PlayerCombatConfig _combatConfig;

        private Rigidbody2D _rb;

        private CharacterMotor _motor;
        private PlayerInputReader _inputReader;
        private CharacterAnimator _characterAnimator;
        private Health _health;
        private PlayerMeleeAttack _attack;
        private ProjectileCaster _projectileCaster;
        private PlayerHitReaction _hitReaction;

        public Vector2 Position => _rb.position;

        public IReadOnlyHealth Health => _health;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            _inputReader = new PlayerInputReader();
            _motor = new CharacterMotor(_rb, _moveConfig);
            _characterAnimator = new CharacterAnimator(_animator, _spriteRenderer);
            _health = new Health(_combatConfig.MaxHP);

            var attackSlow = new SpeedScale();
            var knockback = new Knockback(_combatConfig.Hurt.PushSpeed / _combatConfig.Hurt.Duration);
            _motor.AddModifier(attackSlow);
            _motor.AddModifier(knockback);

            _attack = new PlayerMeleeAttack(this, _combatConfig.Attack, attackSlow);
            _characterAnimator.SetDuration(CharacterAction.Attack, _combatConfig.Attack.Swing.Duration);

            _hitReaction = new PlayerHitReaction(this, _combatConfig.Hurt, knockback, _characterAnimator, _health);
            _characterAnimator.SetDuration(CharacterAction.Hurt, _combatConfig.Hurt.Duration);

            _projectileCaster = new ProjectileCaster(_shootPoint, _combatConfig.Projectile);
        }

        private void OnEnable()
        {
            _health.Reset();
            _inputReader.Enable();
            _characterAnimator.Reset();
            _attack.Reset();

            _hitReaction.Enable();

            _rb.simulated = true;

            _health.Died += OnDied;
            _health.Damaged += OnDamaged;
        }

        private void OnDamaged(DamageInfo info)
        {
            _attack.Reset();
        }

        private void Update()
        {
            _projectileCaster.Tick(Time.deltaTime);
            _attack.Tick(Time.deltaTime);
            _hitReaction.Tick(Time.deltaTime);

            if (_hitReaction.IsActive)
            {
                _motor.SetDirection(Vector2.zero);
                return;
            }

            if (_inputReader.AttackPressed && _attack.TryStart(_characterAnimator.GetFacing()))
            {
                _characterAnimator.PlayAnimation(CharacterAction.Attack, restart: true);
            }

            _motor.SetDirection(_inputReader.GetMoveInput());

            if (!_attack.Swinging)
                _characterAnimator.PlayLocomotion(_motor.Velocity);
        }

        private void FixedUpdate()
        {
            _motor.FixedTick();
        }

        private void OnDied()
        {
            _characterAnimator.PlayAnimation(CharacterAction.Death);
            enabled = false;
            _rb.simulated = false;
        }

        public void TakeDamage(DamageInfo info)
        {
            if (_hitReaction.IsInvulnerable)
                return;

            _health.TakeDamage(info);
        }

        private void OnDisable()
        {
            _inputReader.Disable();
            _motor.Reset();
            _hitReaction.Disable();

            _health.Died -= OnDied;
            _health.Damaged -= OnDamaged;
        }

        private void OnDestroy()
        {
            _inputReader.Dispose();
        }

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                var center = Position + _characterAnimator.GetFacing() * _combatConfig.Attack.Offset;
                Gizmos.DrawWireSphere(center, _combatConfig.Attack.Radius);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_shootPoint.position, _combatConfig.Projectile.Range);
        }
    }
}
