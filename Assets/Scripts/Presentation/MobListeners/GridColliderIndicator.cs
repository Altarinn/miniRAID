using System.Collections.Generic;
using System.Linq;
using miniRAID.Backend;
using UnityEngine;
using UnityEngine.Serialization;

namespace miniRAID
{
    public class GridColliderIndicator : IStateRenderer
    {
        [FormerlySerializedAs("shape")]
        public GridCollider collider;

        public Vector3? losOrigin;
        public Sprite icon;
        public GridOverlay.Types overlayType;

        private GridOverlay instantiatedOverlay;

        public GridColliderIndicator(GridCollider collider, Sprite icon)
        {
            this.collider = collider;
            this.overlayType = GridOverlay.Types.CUSTOM;
            this.icon = icon;
            
            Instantiate();
        }
        
        public GridColliderIndicator(GridCollider collider, GridOverlay.Types overlayType)
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

            IEnumerable<Vector3> grids = Globals.backend.GetColliderMapIntersect(collider);
            if (losOrigin.HasValue)
            {
                grids = grids.Where(p => Globals.backend.HasLineOfSight(losOrigin.Value, p + Globals.half));
            }
            
            if (overlayType == GridOverlay.Types.CUSTOM)
            {
                instantiatedOverlay = Globals.overlayMgr.Instance.FromShape(grids, icon);
            }
            else
            {
                instantiatedOverlay = Globals.overlayMgr.Instance.FromShape(grids, overlayType);
            }
        }

        public void Update(GridCollider shape, Vector3? losOrigin)
        {
            if(shape == null){Destroy(); return;}
            this.collider = shape;
            this.losOrigin = losOrigin;
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