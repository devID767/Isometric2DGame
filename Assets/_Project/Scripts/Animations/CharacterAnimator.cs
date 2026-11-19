using UnityEngine;

namespace Isometric2DGame.Animations
{
    public class CharacterAnimator
    {
        private const float Threshold = 0.01f;

        private readonly Animator _anim;
        private readonly SpriteRenderer _spriteRenderer;

        private bool _facingRight;
        private CharacterAction _action;
        private FacingDirection _direction;

        private int _currentHash;

        public CharacterAnimator(Animator anim, SpriteRenderer spriteRenderer)
        {
            _anim = anim;
            _spriteRenderer = spriteRenderer;
        }

        public void Face(Vector2 direction)
        {
            SelectDirection(direction);
        }

        public void PlayLocomotion(Vector2 velocity)
        {
            if (velocity.sqrMagnitude > Threshold)
            {
                _action = CharacterAction.Walk;

                if (!IsMovingToward(_direction, velocity))
                {
                    SelectDirection(velocity);
                }
            }
            else
            {
                _action = CharacterAction.Idle;
            }

            PlayAnimation(_action);
        }

        public void PlayAnimation(CharacterAction action, bool restart = false)
        {
            _action = action;

            var hash = CharacterAnimations.Get(_action, _direction);
            if (hash != _currentHash)
            {
                _anim.Play(hash);
                _currentHash = hash;
            }
            else if (restart)
            {
                _anim.Play(hash, 0, 0f);
            }
        }

        public void Reset()
        {
            _currentHash = 0;
        }

        private void SelectDirection(Vector2 velocity)
        {
            if (Mathf.Abs(velocity.x) >= Mathf.Abs(velocity.y))
            {
                _direction = FacingDirection.Side;

                _facingRight = velocity.x > 0;
            }
            else
            {
                if (velocity.y > 0)
                    _direction = FacingDirection.Up;
                else
                    _direction = FacingDirection.Down;
            }
            _spriteRenderer.flipX = _direction == FacingDirection.Side && !_facingRight;
        }

        private bool IsMovingToward(FacingDirection direction, Vector2 velocity)
        {
            if (direction == FacingDirection.Up && velocity.y > 0)
                return true;
            else if (direction == FacingDirection.Down && velocity.y < 0)
                return true;
            else if (direction == FacingDirection.Side && _facingRight && velocity.x > 0)
                return true;
            else if (direction == FacingDirection.Side && !_facingRight && velocity.x < 0)
                return true;

            return false;
        }
    }
}