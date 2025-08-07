using miniRAID.Backend;
using UnityEngine;
using UnityEngine.Serialization;

namespace miniRAID
{
    public class GridColliderIndicator : IStateRenderer
    {
        [FormerlySerializedAs("shape")]
        public IGridCollider collider;
        public Sprite icon;
        public GridOverlay.Types overlayType;

        private GridOverlay instantiatedOverlay;

        public GridColliderIndicator(IGridCollider collider, Sprite icon)
        {
            this.collider = collider;
            this.overlayType = GridOverlay.Types.CUSTOM;
            this.icon = icon;
            
            Instantiate();
        }
        
        public GridColliderIndicator(IGridCollider collider, GridOverlay.Types overlayType)
        {
            this.collider = collider;
            this.overlayType = overlayType;
            
            Instantiate();
        }
        
        public void Instantiate()
        {
            if (instantiatedOverlay != null)
            {
                return;
            }
            
            if (overlayType == GridOverlay.Types.CUSTOM)
            {
                instantiatedOverlay = Globals.overlayMgr.Instance.FromShape(Globals.backend.GetColliderMapIntersect(collider), icon);
            }
            else
            {
                instantiatedOverlay = Globals.overlayMgr.Instance.FromShape(Globals.backend.GetColliderMapIntersect(collider), overlayType);
            }
        }

        public void Update(IGridCollider shape)
        {
            if(shape == null){Destroy(); return;}
            this.collider = shape;
            Refresh();
        }

        public virtual void Refresh()
        {
            Destroy();
            Instantiate();
        }

        public virtual void Destroy()
        {
            if (instantiatedOverlay != null)
            {
                GameObject.Destroy(instantiatedOverlay.gameObject);
                instantiatedOverlay = null;
            }
        }

        public GridColliderIndicator Move(Vector3 direction)
        {
            if (instantiatedOverlay != null)
            {
                instantiatedOverlay.transform.position += direction;
            }

            return this;
        }
    }
}