using Isometric2DGame.Movement;
using UnityEngine;

namespace Isometric2DGame.Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private CharacterMoveConfig _moveConfig;

        private CharacterMotor _motor;
        private PlayerInputReader _inputReader;

        private void Awake()
        {
            _inputReader = new PlayerInputReader();
            _motor = new CharacterMotor(GetComponent<Rigidbody2D>(), _moveConfig);
        }

        private void OnEnable()
        {
            _inputReader.Enable();
        }

        private void Update()
        {
            _motor.SetMoveInput(_inputReader.GetMoveInput());
        }

        private void FixedUpdate()
        {
            _motor.FixedTick();
        }

        private void OnDisable()
        {
            _inputReader.Disable();
            _motor.SetMoveInput(Vector2.zero);
        }

        private void OnDestroy()
        {
            _inputReader.Dispose();
        }
    }
}
