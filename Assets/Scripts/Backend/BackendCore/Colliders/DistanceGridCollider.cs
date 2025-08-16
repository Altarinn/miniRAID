using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace miniRAID
{
    [System.Serializable]
    public class DistanceGridCollider : IGridCollider
    {
        [OdinSerialize] public int distance;
        
        public Vector3 Position { get => _position; set => _position = value; }
        [OdinSerialize] private Vector3 _position;
        
        public Consts.Direction Direction { get => _direction; set => _direction = value; }
        [OdinSerialize] private Consts.Direction _direction;

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
            this._position = other._position;
            this._direction = other._direction;
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