using Isometric2DGame.Animations;
using Isometric2DGame.Movement;
using UnityEngine;

namespace Isometric2DGame.MainCharacter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterMoveConfig _moveConfig;

        private Rigidbody2D _rb;

        private CharacterMotor _motor;
        private PlayerInputReader _inputReader;
        private CharacterAnimator _characterAnimator;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            
            _inputReader = new PlayerInputReader();
            _motor = new CharacterMotor(_rb, _moveConfig);
            _characterAnimator = new CharacterAnimator(_animator, _spriteRenderer);
        }

        private void OnEnable()
        {
            _inputReader.Enable();
            _characterAnimator.Reset();
        }

        private void Update()
        {
            _motor.SetDirection(_inputReader.GetMoveInput());
            _characterAnimator.Tick(_motor.Velocity);
        }

        private void FixedUpdate()
        {
            _motor.FixedTick();
        }

        private void OnDisable()
        {
            _inputReader.Disable();
            _motor.Reset();;
        }

        private void OnDestroy()
        {
            _inputReader.Dispose();
        }
    }
}
