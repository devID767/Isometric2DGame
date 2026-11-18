using UnityEngine;

namespace Isometric2DGame.Infrastructure
{
    public class Timer
    {
        private readonly float _initTime;

        private float _currentTime;

        public bool IsDone => _currentTime <= 0;

        public Timer(float time)
        {
            _initTime = time;
            _currentTime = time;
        }

        public void Reset()
        {
            _currentTime = _initTime;
        }

        public void Tick(float deltaTime)
        {
            if (IsDone)
                return;

            _currentTime -= deltaTime;
        }
    }
}