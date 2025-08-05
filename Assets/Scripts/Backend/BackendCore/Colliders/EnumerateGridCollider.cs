using UnityEngine;

namespace miniRAID
{
    public class EnumerateGridCollider__ : IGridCollider
    {
        public EnumerateGridCollider shape;
        
        public bool Overlaps(IGridCollider other)
        {
            if (other is EnumerateGridCollider)
                return ColliderOverlapTests.Overlaps(this, (EnumerateGridCollider)other);
            
            throw new System.NotImplementedException();
        }

        public void SetPosition(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public void SetDirection(Consts.Direction dirc)
        {
            throw new System.NotImplementedException();
        }
    }
}