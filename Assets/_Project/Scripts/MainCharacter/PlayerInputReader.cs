using System;
using UnityEngine;

namespace Isometric2DGame.MainCharacter
{
    public class PlayerInputReader : IDisposable
    {
        private readonly InputSystem_Actions _input;

        public bool AttackPressed => _input.Player.Attack.WasPressedThisFrame();

        public PlayerInputReader()
        {
            _input = new InputSystem_Actions();
        }

        public void Enable()
        {
            _input.Player.Enable();
        }

        public Vector2 GetMoveInput()
        {
            var input = Vector2.ClampMagnitude(_input.Player.Move.ReadValue<Vector2>(), 1f);
            return input;
        }

        public void Disable()
        {
            _input.Player.Disable();
        }

        public void Dispose()
        {
            _input.Dispose();
        }
    }
}
