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
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3
        };

        public static Vector3Int[] directionVectors = new Vector3Int[4]
        {
            Vector3Int.forward,
            Vector3Int.left,
            Vector3Int.back,
            Vector3Int.right,
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

        public static GridShape Combine(GridShape a, GridShape b) => Combine(a.shape, b.shape);
        
        public static GridShape Combine(HashSet<Vector3Int> a, HashSet<Vector3Int> b)
        {
            var result = new GridShape();

            foreach (var p in a)
            {
                result.AddGrid(p);
            }

            foreach (var p in b)
            {
                result.AddGrid(p);
            }

            return result;
        }

        public static GridShape Negate(GridShape a)
        {
            var result = new GridShape();

            foreach (var p in Globals.backend.GetAllMapGridPositions().ToIEnumerable())
            {
                if(a.shape.Contains(p))
                    continue;
                
                result.AddGrid(p);
            }

            return result;
        }
    }
}
