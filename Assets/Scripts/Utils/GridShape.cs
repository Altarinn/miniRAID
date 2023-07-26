using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace miniRAID
{
    [ColoredBox("#7fd")]
    public class GridShape
    {
        public enum Direction
        {
            Up,
            Left,
            Down,
            Right
        };

        public HashSet<Vector3Int> shape;

        // Editor only
        public int canvasSize;

        public Vector3Int position;
        public Direction direction;

        public GridShape() 
        { 
            this.shape = new HashSet<Vector3Int>();
        }

        public GridShape(Vector3Int shape)
        {
            this.shape = new HashSet<Vector3Int>();
            this.shape.Add(shape);
        }

        public GridShape(IEnumerable<Vector3Int> shape)
        {
            this.shape = new HashSet<Vector3Int>(shape);
        }

        public GridShape(HashSet<Vector3Int> shape)
        {
            this.shape = shape;
        }

        public void AddGrid(Vector3Int rPos)
        {
            shape.Add(rPos);
        }
        
        public void RemoveGrid(Vector3Int rPos)
        {
            shape.Remove(rPos);
        }

        public HashSet<Vector3Int> ApplyTransform()
        {
            HashSet<Vector3Int> result = new HashSet<Vector3Int>();

            foreach (var p in shape)
            {
                switch (direction)
                {
                    case Direction.Up:
                        result.Add(p + position);
                        break;
                    case Direction.Down:
                        result.Add(new Vector3Int(p.x, 0, -p.z) + position);
                        break;
                    case Direction.Left:
                        result.Add(new Vector3Int(-p.z, 0, p.x) + position);
                        break;
                    case Direction.Right:
                        result.Add(new Vector3Int(p.z, 0, -p.x) + position);
                        break;
                }
            }

            return result;
        }
    }
}
