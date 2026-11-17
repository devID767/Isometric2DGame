using System;
using UnityEngine;

namespace Isometric2DGame.Animations
{
    public static class CharacterAnimations
    {
        private static readonly int[,] Hashes = BuildHashes();

        public static int Get(CharacterAction action, FacingDirection direction)
        {
            return Hashes[(int)action, (int)direction];
        }

        private static int[,] BuildHashes()
        {
            var actions = (CharacterAction[])Enum.GetValues(typeof(CharacterAction));
            var directions = (FacingDirection[])Enum.GetValues(typeof(FacingDirection));
            var result = new int[actions.Length, directions.Length];

            foreach (var action in actions)
            {
                foreach (var direction in directions)
                {
                    result[(int)action, (int)direction] = Animator.StringToHash($"{action}_{direction}");
                }
            }

            return result;
        }
    }
}