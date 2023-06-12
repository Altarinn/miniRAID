using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace miniRAID.Spells
{
    [ParameterDefaultName("target")]

    public class SpellTarget
    {
        public List<Vector3Int> targetPos = new List<Vector3Int>();

        public SpellTarget() { }

        public SpellTarget(Vector3Int point)
        {
            targetPos.Add(point);
        }

        public SpellTarget(IEnumerable<Vector3Int> point)
        {
            targetPos.AddRange(point);
        }
    }
}