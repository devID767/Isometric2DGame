using System;
using UnityEngine;

namespace Isometric2DGame.Infrastructure
{
    public class ComponentPool<T> where T : Component
    {
        private readonly UnityEngine.Pool.ObjectPool<T> _pool;
        private readonly T _prefab;

        public int CountActive => _pool.CountActive;

        public ComponentPool(T prefab)
        {
            _prefab = prefab;

            _pool = new UnityEngine.Pool.ObjectPool<T>(Create, null, OnRelease, OnDestroyAction);
        }

        public T Get(Action<T> setup = null)
        {
            var item = _pool.Get();
            setup?.Invoke(item);
            item.gameObject.SetActive(true);
            return item;
        }

        public void Release(T item)
        {
            _pool.Release(item);
        }

        private T Create()
        {
            var item = UnityEngine.Object.Instantiate(_prefab);
            item.gameObject.SetActive(false);
            return item;
        }

        private void OnRelease(T item)
        {
            item.gameObject.SetActive(false);
        }

        private void OnDestroyAction(T item)
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }
}
