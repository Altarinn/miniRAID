using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace miniRAID
{
    [System.Serializable]
    public class DistanceGridCollider : GridCollider
    {
        [OdinSerialize] public int distance;

        public DistanceGridCollider()
        {
            this.distance = 1;
        }
        
        public DistanceGridCollider(int distance) : base()
        {
            this.distance = distance;
        }
        
        public DistanceGridCollider(DistanceGridCollider other) : base(other)
        {
            this.distance = other.distance;
        }

        public override bool OverlapsAllowDuplicate(GridCollider other)
        {
            if (other is DistanceGridCollider otherDistance)
                return ColliderOverlapTests.Overlaps(otherDistance, this);

            if (other is EnumerateGridCollider otherEnum)
                return ColliderOverlapTests.Overlaps(otherEnum, this);
            
            throw new System.NotImplementedException($"Overlap not implemented for {other.GetType()}");
        }

        public bool OverlapsGrid(Vector3Int gridCell)
        {
            return Consts.Distance(Position, gridCell) <= distance;
        }

        public override GridCollider ShallowClone()
        {
            return new DistanceGridCollider(this);
        }

        protected override GridCollider Clone()
        {
            return new DistanceGridCollider(this);
        }

        public override IEnumerator<Vector3Int> GetEnumerator()
        {
            Vector3Int center = Vector3Int.FloorToInt(Position);
            
            for (int x = -distance; x <= distance; x++)
            {
                for (int y = -distance; y <= distance; y++)
                {
                    for (int z = -distance; z <= distance; z++)
                    {
                        Vector3Int point = center + new Vector3Int(x, y, z);
                        if (Consts.Distance(Position, point) <= distance)
                        {
                            yield return point;
                        }
                    }
                }
            }
        }
    }
}