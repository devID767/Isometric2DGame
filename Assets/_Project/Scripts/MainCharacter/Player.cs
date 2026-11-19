using System;
using Isometric2DGame.Animations;
using Isometric2DGame.Combat;
using Isometric2DGame.Movement;
using UnityEngine;

namespace Isometric2DGame.MainCharacter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, ITarget
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

        public Vector2 Position => _rb.position;

        public bool IsAlive => _health.IsAlive;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            _inputReader = new PlayerInputReader();
            _motor = new CharacterMotor(_rb, _moveConfig);
            _characterAnimator = new CharacterAnimator(_animator, _spriteRenderer);
            _health = new Health(_combatConfig.MaxHP);
        }

        private void OnEnable()
        {
            _health.Reset();
            _inputReader.Enable();
            _characterAnimator.Reset();

            _health.Died += OnDied;
        }

        private void Update()
        {
            _motor.SetDirection(_inputReader.GetMoveInput());
            _characterAnimator.PlayLocomotion(_motor.Velocity);
        }

        private void FixedUpdate()
        {
            _motor.FixedTick();
        }

         private void OnDied()
        {
            enabled = false;
        }

        private void OnDisable()
        {
            _inputReader.Disable();
            _motor.Reset();

            _health.Died -= OnDied;
        }

        private void OnDestroy()
        {
            _inputReader.Dispose();
        }

        public void TakeDamage(int value)
        {
            _health.TakeDamage(value);
            Debug.Log($"Take damage{value}");
        }
    }
}
