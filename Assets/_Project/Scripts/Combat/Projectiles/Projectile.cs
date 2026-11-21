using System;
using UnityEngine;

namespace Isometric2DGame.Combat.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _radius = 0.12f;

        private Rigidbody2D _rb;

        private int _damage;
        private float _lifeTime;
        private Vector2 _velocity;
        private LayerMask _hitMask;

        private Action<Projectile> _release;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 pos, Vector2 dir, int damage, float speed, float lifetime, LayerMask hitMask, Action<Projectile> release)
        {
            _damage = damage;
            _lifeTime = lifetime;
            _release = release;
            _hitMask = hitMask;

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(pos, Quaternion.Euler(0f, 0f, angle));
            _rb.position = pos;
            _rb.rotation = angle;
            _velocity = dir * speed;
        }

        private void Update()
        {
            if (_lifeTime > 0)
            {
                _lifeTime -= Time.deltaTime;
            }
            else
            {
                Despawn();
            }
        }

        private void FixedUpdate()
        {
            var step = _velocity * Time.fixedDeltaTime;
            var hit = Physics2D.CircleCast(_rb.position, _radius, _velocity, step.magnitude, _hitMask);
            if (hit)
            {
                OnHit(hit.collider);
                return;
            }
            _rb.MovePosition(_rb.position + step);
        }

        private void OnHit(Collider2D collision)
        {
            if (collision.attachedRigidbody != null && collision.attachedRigidbody.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(new DamageInfo(_damage, _rb.position));
            }

            Despawn();
        }

        private void Despawn()
        {
            _release?.Invoke(this);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
