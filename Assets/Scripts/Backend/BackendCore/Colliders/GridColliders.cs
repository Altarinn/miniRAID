using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace miniRAID
{
    public interface IGridCollider : ICloneable, IEnumerable<Vector3Int>
    {
        bool Overlaps(IGridCollider other);

        [OdinSerialize]
        Vector3 Position { get; set; }
        
        [OdinSerialize]
        Consts.Direction Direction { get; set; }

        IGridCollider ShallowClone();
    }

    // TODO: Refine me?
    public class PointCollider : EnumerateGridCollider
    {
        public PointCollider() : base(new GridShape(Vector3Int.zero))
        { }
        
        public PointCollider(PointCollider other) : base(other)
        { }
    }

    public static class ColliderOverlapTests
    {
        // <summary>
        /// Checks if two BoundsInt intersect when positioned at their respective offset positions.
        /// </summary>
        /// <param name="bounds1">The first BoundsInt</param>
        /// <param name="offset1">The world position offset for the first bounds</param>
        /// <param name="bounds2">The second BoundsInt</param>
        /// <param name="offset2">The world position offset for the second bounds</param>
        /// <returns>True if the bounds intersect, false otherwise</returns>
        public static bool IntersectsBounds(EnumerateGridCollider a, EnumerateGridCollider b)
        {
            var bounds1 = Consts.Rotate(a.shape.Bounds, a.Direction);
            var bounds2 = Consts.Rotate(b.shape.Bounds, b.Direction);
            
            var offset1 = a.Position;
            var offset2 = b.Position;
            
            // Convert BoundsInt to world space Bounds using the offsets
            Bounds worldBounds1 = new Bounds(
                center: new Vector3(bounds1.center.x, bounds1.center.y, bounds1.center.z) + offset1,
                size: new Vector3(bounds1.size.x, bounds1.size.y, bounds1.size.z)
            );
        
            Bounds worldBounds2 = new Bounds(
                center: new Vector3(bounds2.center.x, bounds2.center.y, bounds2.center.z) + offset2,
                size: new Vector3(bounds2.size.x, bounds2.size.y, bounds2.size.z)
            );
        
            // Use Unity's built-in intersection check
            return worldBounds1.Intersects(worldBounds2);
        }
        
        public static bool Overlaps(EnumerateGridCollider a, EnumerateGridCollider b)
        {
            if (!IntersectsBounds(a, b))
                return false;

            var (smaller, larger) = a.shape.shape.Count <= b.shape.shape.Count ? (a, b) : (b, a);
    
            // Rotate larger set once and store
            var largerRotated = new HashSet<Vector3Int>();
            foreach (var pos in larger.shape.shape)
            {
                largerRotated.Add(Consts.Rotate(pos, larger.Direction));
            }
    
            // Handle position offset
            Vector3 relativePos = smaller.Position - larger.Position;
    
            // Check smaller set with duplication logic - no temporary HashSets
            foreach (var pos in EnumerateGridCollider.OverlappedPoints(smaller.shape, relativePos, smaller.Direction))
            {
                if (largerRotated.Contains(pos))
                    return true;
            }
    
            return false;
        }

        public static bool Overlaps(EnumerateGridCollider a, DistanceGridCollider b)
        {
            foreach (var gridPos in a)
            {
                if (b.OverlapsGrid(gridPos))
                    return true;
            }
            return false;
        }
        
        public static bool Overlaps(DistanceGridCollider a, DistanceGridCollider b)
        {
            int centerDistance = Consts.Distance(a.Position, b.Position);
            return centerDistance <= (a.distance + b.distance);
        }
    }
}