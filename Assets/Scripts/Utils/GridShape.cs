using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace miniRAID
{
    public class GridShape
    {
        public enum Direction
        {
            Up,
            Left,
            Down,
            Right
        };

        public HashSet<Vector2Int> shape;

        public Vector2Int position;
        public Direction direction;

        public GridShape() 
        { 
            this.shape = new HashSet<Vector2Int>();
        }

        public GridShape(Vector2Int shape)
        {
            this.shape = new HashSet<Vector2Int>();
            this.shape.Add(shape);
        }

        public GridShape(IEnumerable<Vector2Int> shape)
        {
            this.shape = new HashSet<Vector2Int>(shape);
        }

        public GridShape(HashSet<Vector2Int> shape)
        {
            this.shape = shape;
        }

        public void AddGrid(Vector2Int rPos)
        {
            shape.Add(rPos);
        }

        public HashSet<Vector2Int> ApplyTransform()
        {
            HashSet<Vector2Int> result = new HashSet<Vector2Int>();

            foreach (var p in shape)
            {
                switch (direction)
                {
                    case Direction.Up:
                        result.Add(p + position);
                        break;
                    case Direction.Down:
                        result.Add(new Vector2Int(p.x, -p.y) + position);
                        break;
                    case Direction.Left:
                        result.Add(new Vector2Int(-p.y, p.x) + position);
                        break;
                    case Direction.Right:
                        result.Add(new Vector2Int(p.y, -p.x) + position);
                        break;
                }
            }

            return result;
        }
    }
}
