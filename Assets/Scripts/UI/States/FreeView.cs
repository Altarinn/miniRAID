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
            MobRenderer mobRenderer = Databackend.GetSingleton().GetMap(ui.cursor.position.x, ui.cursor.position.y, ui.cursor.position.z)?.mob?.mobRenderer;
            if (mobRenderer)
            {
                if (mobRenderer.data.isControllable)
                {
                    Debug.Log($"Unit selected: {mobRenderer.gameObject.name}");
                    ui.characterFocusVCam.Follow = mobRenderer.transform;
                    ui.characterFocusVCam.enabled = false;
                    ui.characterFocusVCam.transform.position = 
                        new Vector3(
                            mobRenderer.transform.position.x,
                            mobRenderer.transform.position.y,
                            ui.characterFocusVCam.transform.position.z);
                    ui.characterFocusVCam.enabled = true;
                    ui.characterFocusVCam.Priority = 100; // Activate character camera
                    ui.EnterState(new UnitMenu(mobRenderer), true);
                }
                else
                {
                    Globals.debugMessage.AddMessage($"{mobRenderer.name}已经行动完毕！");
                    Debug.Log($"Unit slept!");
                }
            }
        }

        public override void PointAtGrid(Vector3Int gridPos)
        {
            base.PointAtGrid(gridPos);

            if(Globals.backend.InMap(gridPos))
            {
                var mob = Globals.backend.GetMap(gridPos.x, gridPos.y, gridPos.z).mob?.mobRenderer;
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
