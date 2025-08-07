using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID
{
    public class EnumerateGridCollider : IGridCollider
    {
        public GridShape shape;
        
        public Vector3 Position { get; set; }
        public Consts.Direction Direction { get; set; }

        public EnumerateGridCollider(GridShape shape)
        {
            this.shape = shape;
        }

        public EnumerateGridCollider(EnumerateGridCollider other)
        {
            shape = new GridShape(other.shape);
            Position = other.Position;
            Direction = other.Direction;
        }

        public bool Overlaps(IGridCollider other)
        {
            if (other is EnumerateGridCollider)
                return ColliderOverlapTests.Overlaps(this, (EnumerateGridCollider)other);
            
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