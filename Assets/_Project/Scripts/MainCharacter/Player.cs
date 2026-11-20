using Isometric2DGame.Animations;
using Isometric2DGame.Combat;
using Isometric2DGame.Movement;
using UnityEngine;

namespace Isometric2DGame.MainCharacter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, ITarget, IHealthOwner
    {
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
        private PlayerHurt _hurt;

        public Vector2 Position => _rb.position;

        public bool IsAlive => _health.Data.IsAlive;

        public IReadOnlyHealth Health => _health;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            _inputReader = new PlayerInputReader();
            _motor = new CharacterMotor(_rb, _moveConfig);
            _characterAnimator = new CharacterAnimator(_animator, _spriteRenderer);
            _health = new Health(_combatConfig.MaxHP);

            _attack = new PlayerMeleeAttack(this, _combatConfig.Attack);
            _characterAnimator.SetDuration(CharacterAction.Attack, _combatConfig.Attack.Swing.Duration);

            _hurt = new PlayerHurt(_combatConfig.HurtDuration, _health, _characterAnimator, _attack);
        }

        private void OnEnable()
        {
            _health.Reset();
            _inputReader.Enable();
            _characterAnimator.Reset();
            _attack.Reset();

            _health.Died += OnDied;

            _hurt.Enable();

            _rb.simulated = true;
        }

        private void Update()
        {
            _attack.Tick(Time.deltaTime);
            _hurt.Tick(Time.deltaTime);

            if (_hurt.IsActive)
            {
                _motor.SetDirection(Vector2.zero);
                return;
            }

            if (_inputReader.AttackPressed && _attack.TryStart(_characterAnimator.GetFacing()))
            {
                _characterAnimator.PlayAnimation(CharacterAction.Attack, restart: true);
            }

            var input = _inputReader.GetMoveInput();
            if (_attack.Swinging)
            {
                _motor.SetDirection(input, _combatConfig.Attack.MoveSpeedScale);
            }
            else
            {
                _motor.SetDirection(input);
                _characterAnimator.PlayLocomotion(_motor.Velocity);
            }
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

        public void TakeDamage(int value)
        {
            _health.TakeDamage(value);
        }

        private void OnDisable()
        {
            _inputReader.Disable();
            _motor.Reset();
            _hurt.Disable();

            _health.Died -= OnDied;
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
        }
    }
}
