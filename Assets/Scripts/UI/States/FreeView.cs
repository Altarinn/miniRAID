using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace miniRAID.UI
{
    public class FreeView : UIState
    {
        public FreeView()
        {
            stateStr = "FreeView";
        }

        public override void Submit(InputValue input)
        {
            base.Submit(input);
            Mob mob = Databackend.GetSingleton().getMap(ui.cursor.position.x, ui.cursor.position.y).mob;
            if (mob)
            {
                if (mob.isActive)
                {
                    Debug.Log($"Unit selected: {mob.gameObject.name}");
                    ui.characterFocusVCam.Follow = mob.transform;
                    ui.characterFocusVCam.enabled = false;
                    ui.characterFocusVCam.transform.position = 
                        new Vector3(
                            mob.transform.position.x,
                            mob.transform.position.y,
                            ui.characterFocusVCam.transform.position.z);
                    ui.characterFocusVCam.enabled = true;
                    ui.characterFocusVCam.Priority = 100; // Activate character camera
                    ui.EnterState(new UnitMenu(mob), true);
                }
                else
                {
                    Globals.debugMessage.Instance.Message($"{mob.name}已经行动完毕！");
                    Debug.Log($"Unit slept!");
                }
            }
        }

        public override void PointAtGrid(Vector2Int gridPos)
        {
            base.PointAtGrid(gridPos);

            if(Globals.backend.InMap(gridPos))
            {
                var mob = Globals.backend.getMap(gridPos.x, gridPos.y).mob;
                if (mob != null)
                {
                    ui.ShowMainMobStats(mob);
                }
                else
                {
                    ui.HideMainMobStats();
                }
            }
        }

        public override void Cancel(InputValue input)
        {
            // Do nothing, as FreeView is the root state.
            return;
        }
    }
}
