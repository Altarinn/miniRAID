using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace miniRAID.UI
{
    public class GridShapeCursor : GridColliderIndicator
    {
        public Vector3Int GridPos => Databackend.BackendToGridPos(collider.Position);

        public Vector3 Position
        {
            get => collider.Position;
            set
            {
                collider.Position = value;
                Refresh();
            }
        }

        public Consts.Direction Direction
        {
            get => collider.Direction;
            set
            {
                collider.Direction = value;
                Refresh();
            }
        }
        
        public GridShapeCursor(IGridCollider collider, Sprite icon) : base(collider, icon)
        {
        }

        public GridShapeCursor(IGridCollider collider, GridOverlay.Types overlayType) : base(collider, overlayType)
        {
        }
        
        public void ChangeCollider(IGridCollider collider)
        {
            var p = this.collider.Position;
            var d = this.collider.Direction;
            
            this.collider = collider;

            this.collider.Position = p;
            this.collider.Direction = d;
            
            Refresh();
        }
    }
}
