using miniRAID.Backend;
using UnityEngine;

namespace miniRAID
{
    public class GridShapeIndicator : IStateRenderer
    {
        public GridShape shape;
        public Sprite icon;
        public GridOverlay.Types overlayType;

        private GridOverlay instantiatedOverlay;

        public GridShapeIndicator(GridShape shape, Sprite icon)
        {
            this.shape = shape;
            this.overlayType = GridOverlay.Types.CUSTOM;
            this.icon = icon;
            
            Instantiate();
        }
        
        public GridShapeIndicator(GridShape shape, GridOverlay.Types overlayType)
        {
            this.shape = shape;
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
                instantiatedOverlay = Globals.overlayMgr.Instance.FromShape(shape, icon);
            }
            else
            {
                instantiatedOverlay = Globals.overlayMgr.Instance.FromShape(shape, overlayType);
            }
        }

        public void Update(GridShape shape)
        {
            this.shape = shape;
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
            }
        }

        public GridShapeIndicator Move(Vector3 direction)
        {
            if (instantiatedOverlay != null)
            {
                instantiatedOverlay.transform.position += direction;
            }

            return this;
        }
    }
}