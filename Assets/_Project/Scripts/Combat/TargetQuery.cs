using System.Collections.Generic;
using UnityEngine;

namespace Isometric2DGame.Combat
{
    public class TargetQuery
    {
        private readonly List<Collider2D> _results;
        private readonly ContactFilter2D _filter;
        private readonly HashSet<ITarget> _seen;

        private readonly List<ITarget> _targets;

        public TargetQuery(LayerMask layerMask)
        {
            _results = new List<Collider2D>();
            _filter = new ContactFilter2D();
            _filter.SetLayerMask(layerMask);
            _filter.useTriggers = false;

            _seen = new HashSet<ITarget>();

            _targets = new List<ITarget>();
        }

        public IReadOnlyList<ITarget> Overlap(Vector2 center, float radius)
        {
            _targets.Clear();
            _seen.Clear();

            var count = Physics2D.OverlapCircle(center, radius, _filter, _results);

            for (int i = 0; i < count; i++)
            {
                var targetRb = _results[i].attachedRigidbody;
                if (targetRb != null && targetRb.TryGetComponent(out ITarget target))
                {
                    if (!_seen.Contains(target))
                    {
                        _targets.Add(target);
                        _seen.Add(target);
                    }
                }
            }
            return _targets;
        }
    }
}