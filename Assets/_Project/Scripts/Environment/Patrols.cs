using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Isometric2DGame.Environment
{

    public class Patrols : MonoBehaviour
    {
        [SerializeField] private List<Transform> _points;

        public List<Vector2> GetPoints() => _points.Select(patrol => (Vector2)patrol.transform.position).ToList();
    }
}
