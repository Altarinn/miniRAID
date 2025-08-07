using UnityEngine;

namespace miniRAID
{
    public interface IGridCollider
    {
        bool Overlaps(IGridCollider other);

        Vector3 Position { get; set; }
        Consts.Direction Direction { get; set; }
    }

    public static class ColliderOverlapTests
    {
        public static bool Overlaps(EnumerateGridCollider a, EnumerateGridCollider b)
        {
            var aa = a.shape.ApplyTransform();
            var bb = (b as EnumerateGridCollider).shape.ApplyTransform();
            return aa.Overlaps(bb);
        }
    }
}