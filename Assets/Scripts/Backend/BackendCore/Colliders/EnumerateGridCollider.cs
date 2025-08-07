using System;
using UnityEngine;

namespace miniRAID
{
    public class EnumerateGridCollider : IGridCollider, ICloneable
    {
        public GridShape shape;
        
        public Vector3 Position { get => shape.position; set => shape.position = value; }
        public Consts.Direction Direction { get; set; }

        public EnumerateGridCollider()
        { }

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
    }
}