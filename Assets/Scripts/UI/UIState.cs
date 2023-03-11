using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace miniRAID.UI
{
    public class UIState
    {
        [HideInInspector]
        public string stateStr;

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

        public virtual void PointAtGrid(Vector2Int gridPos) { }

        public virtual void Cancel(InputValue input)
        {
            ui.BackState();
        }

        //public void WaitForAnimation() => ui.EnterState(new WaitAnimState(), true);
        //public void AnimationFinish() => ui.BackState();
    }
}
