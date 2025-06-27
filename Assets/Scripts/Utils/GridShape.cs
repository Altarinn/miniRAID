using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;
using UnityEngine;

namespace miniRAID
{
    [ColoredBox("#7fd")]
    [MemoryPackable]
    public class GridShape : ICloneable
    {

        public HashSet<Vector3Int> shape;

        // Editor only
        public int canvasSize;

        public Vector3Int position;
        public Consts.Direction direction;

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

        public GridShape(GridShape from)
        {
            position = from.position;
            direction = from.direction;
            canvasSize = from.canvasSize;

            // Better ways to copy this?
            // https://stackoverflow.com/questions/3927789/efficient-way-to-clone-a-hashsett
            // > in .NET 4.7.2, can clone effectively via new HashSet(from, from.Comparer);.
            // Do we have .NET 4.7.2?
            shape = new HashSet<Vector3Int>(from.shape.ToList());
        }
        
        public object Clone()
        {
            return new GridShape(this);
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
                    case Consts.Direction.Up:
                        result.Add(p + position);
                        break;
                    case Consts.Direction.Down:
                        result.Add(new Vector3Int(p.x, 0, -p.z) + position);
                        break;
                    case Consts.Direction.Left:
                        result.Add(new Vector3Int(-p.z, 0, p.x) + position);
                        break;
                    case Consts.Direction.Right:
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
