using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace miniRAID
{
    [System.Serializable]
    public class EnumerateGridCollider : IGridCollider
    {
        [OdinSerialize] public GridShape shape;
        
        public Vector3 Position { get => _position; set => _position = value; }
        [OdinSerialize] private Vector3 _position;
        
        public Consts.Direction Direction { get => _direction; set => _direction = value; }
        [OdinSerialize] private Consts.Direction _direction;

        public EnumerateGridCollider()
        {
            this.shape = new GridShape();
        }
        
        public EnumerateGridCollider(GridShape shape)
        {
            this.shape = shape;
        }

        public EnumerateGridCollider(EnumerateGridCollider other, bool shallow = false)
        {
            if (shallow)
            {
                shape = other.shape;
            }
            else
            {
                shape = new GridShape(other.shape);
            }
            
            _position = other._position;
            _direction = other._direction;
        }
        
        public IGridCollider ShallowClone()
        {
            return new EnumerateGridCollider(this, true);
        }

        public bool Overlaps(IGridCollider other)
        {
            if (other is EnumerateGridCollider ec)
                return ColliderOverlapTests.Overlaps(this, ec);

            if (other is DistanceGridCollider dc)
                return ColliderOverlapTests.Overlaps(this, dc);
            
            throw new System.NotImplementedException();
        }

        public object Clone()
        {
            return new EnumerateGridCollider(this);
        }

        public IEnumerator<Vector3Int> GetEnumerator()
        {
            return new HashSet<Vector3Int>(OverlappedPoints(shape, Position, Direction)).GetEnumerator();
        }

        public static IEnumerable<Vector3Int> OverlappedPoints(GridShape shape, Vector3 position, Consts.Direction dirc)
        {
            Vector3Int intPos = Vector3Int.FloorToInt(position);
            Vector3 fractPos = position - intPos;
            
            foreach (var pos in shape.shape)
            {
                Vector3Int rotatedPos = Consts.Rotate(pos, dirc);
        
                // Check all necessary duplicate positions directly
                for (int dx = 0; dx < 2 - (fractPos.x == 0 ? 1 : 0); dx++)
                {
                    for (int dy = 0; dy < 2 - (fractPos.y == 0 ? 1 : 0); dy++)
                    {
                        for (int dz = 0; dz < 2 - (fractPos.z == 0 ? 1 : 0); dz++)
                        {
                            Vector3Int checkPos = rotatedPos + new Vector3Int(dx, dy, dz) + intPos;
                            yield return checkPos;
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