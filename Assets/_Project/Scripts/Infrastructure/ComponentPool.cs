using UnityEngine;

namespace Isometric2DGame.Infrastructure
{
    public class ComponentPool<T> where T : Component
    {
        private readonly UnityEngine.Pool.ObjectPool<T> _pool;
        private readonly T _prefab;

        public ComponentPool(T prefab)
        {
            _prefab = prefab;

            _pool = new UnityEngine.Pool.ObjectPool<T>(Create, OnGet, OnRelease, OnDestroyAction);
        }

        public T Get()
        {
            return _pool.Get();
        }

        public void Release(T projectile)
        {
            _pool.Release(projectile);
        }

        private T Create()
        {
            return Object.Instantiate(_prefab);
        }

        private void OnGet(T item)
        {
            item.gameObject.SetActive(true);
        }

        private void OnRelease(T item)
        {
            item.gameObject.SetActive(false);
        }

        private void OnDestroyAction(T item)
        {
            Object.Destroy(item.gameObject);
        }
    }
}
