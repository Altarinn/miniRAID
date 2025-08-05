using UnityEngine;

namespace miniRAID
{
    public interface IGridCollider
    {
        bool Overlaps(IGridCollider other);

        void SetPosition(Vector3 position);
        void SetDirection(Consts.Direction dirc);
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