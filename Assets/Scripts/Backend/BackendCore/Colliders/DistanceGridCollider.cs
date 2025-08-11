using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID
{
    public class DistanceGridCollider : IGridCollider
    {
        public int distance;
        
        public Vector3 Position { get; set; }
        public Consts.Direction Direction { get; set; }

        public DistanceGridCollider()
        {
            this.distance = 1;
        }
        
        public DistanceGridCollider(int distance)
        {
            this.distance = distance;
        }
        
        public DistanceGridCollider(DistanceGridCollider other)
        {
            this.distance = other.distance;
            this.Position = other.Position;
            this.Direction = other.Direction;
        }

        public bool Overlaps(IGridCollider other)
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

        public IGridCollider ShallowClone()
        {
            return new DistanceGridCollider(this);
        }

        public object Clone()
        {
            return new DistanceGridCollider(this);
        }

        public IEnumerator<Vector3Int> GetEnumerator()
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}