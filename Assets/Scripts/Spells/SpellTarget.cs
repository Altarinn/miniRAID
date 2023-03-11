using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace miniRAID.Spells
{
    [ParameterDefaultName("target")]

    public class SpellTarget
    {
        public List<Vector2Int> targetPos = new List<Vector2Int>();

        public SpellTarget() { }

        public SpellTarget(Vector2Int point)
        {
            targetPos.Add(point);
        }

        public SpellTarget(IEnumerable<Vector2Int> point)
        {
            targetPos.AddRange(point);
        }
    }
}