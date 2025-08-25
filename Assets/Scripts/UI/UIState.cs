using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using miniRAID.UIElements;

namespace miniRAID.UI
{
    public class UIState
    {
        [HideInInspector]
        public string stateStr;

        public virtual bool AllowFreeNavigation => true;

        GridUI _ui;
        protected GridUI ui
        {
            get
            {
                if (_ui == null)
                {
                    _ui = Globals.ui.Instance;
                }
                return _ui;
            }
        }

        public bool enabled
        {
            get
            {
                return ui.currentState == this;
            }
        }

        public virtual void OnStateEnter() { }

        public virtual void OnStateExit() { }
        public virtual void OnStateDestroyed() { }

        public virtual void Submit(InputValue input) { }

        public virtual void PointAtGrid(Vector3Int gridPos) { }

        public virtual void MoveCursor(Vector3Int movement)
        {
            Vector3Int currentPos = Databackend.BackendToGridPos(ui.cursor.Position);
            Vector3Int targetXZ = currentPos + movement;
            
            // Find nearest valid Y position
            Vector3Int? validPos = ui.FindNearestWalkablePosition(targetXZ, currentPos.y);
            if (validPos.HasValue)
            {
                ui.cursor.Position = Globals.backend.GridToBackendFloorPos(validPos.Value);
                PointAtGrid(validPos.Value);
            }
        }

        public virtual void Cancel(InputValue input)
        {
            ui.BackState();
        }

        /// <summary>
        /// Configure UI Toolkit navigation behavior for this state.
        /// Default behavior: Disable all UI Toolkit navigation to prevent conflicts with GridUI.
        /// </summary>
        /// <param name="combatView">The CombatView to configure</param>
        public virtual void ConfigureUIToolkitNavigation(CombatView combatView)
        {
            // Default: Disable ALL UI Toolkit navigation
            combatView.SetNavigationPolicy(UINavigationPolicy.DisableAll);
        }

        //public void WaitForAnimation() => ui.EnterState(new WaitAnimState(), true);
        //public void AnimationFinish() => ui.BackState();
    }
}
