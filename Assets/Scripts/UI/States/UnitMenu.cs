using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace miniRAID.UI
{
    public class UnitMenu : UIState
    {
        public enum MenuSubState
        {
            Root,
            Action,
        }

        MenuSubState subState;

        public UIMenu_UIContainer menu;
        MobRenderer currentUnit;

        bool keepCameraPosition = false;

        public UnitMenu(MobRenderer mobRenderer)
        {
            stateStr = $"UnitMenu: {mobRenderer.name}";

            currentUnit = mobRenderer;
            subState = MenuSubState.Root;

            keepCameraPosition = false;

            if (currentUnit)
            {
                currentUnit.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                ui.ShowMainMobStats(currentUnit);
            }
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            currentUnit.data.RecalculateStats();

            switch(subState)
            {
                case MenuSubState.Root:
                    PrepareTopMenu();
                    break;
                case MenuSubState.Action:
                    PrepareSpellMenu();
                    break;
            }

            //ui.uimenu_uicontainer.Show();
        }

        public override void OnStateDestroyed()
        {
            base.OnStateDestroyed();
            if (currentUnit)
            {
                currentUnit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }

            //ui.uimenu_uicontainer.Clear();
            ui.characterFocusVCam.Priority = 0; // Deactivate character camera

            // Set camera position to current place if keep position (used action)
            if(keepCameraPosition)
            {
                ui.mainVCam.transform.position = ui.characterFocusVCam.transform.position;
                keepCameraPosition = false;
            }
            ui.combatView.menu.ClearMenu();
            ui.HideMainMobStats();
        }

        private void PrepareTopMenu()
        {
            //ui.uimenu_uicontainer.Clear();

            // TODO: Move as ActionDataSO.
            //var moveAction = currentUnit.GetAction<Actions.Move>();
            //if (moveAction != null)
            //{
            //    // TODO: Check moveable ... finished ?
            //    AddActionEntry(moveAction, true);
            //}

            // Get Mob-specific Menu
            Debug.LogWarning("Mob - ShowMobSpecificMenu unimplemented");
            //currentUnit.ShowMenu(this, ui.uimenu_uicontainer);

            List<miniRAID.UIElements.UnitMenuController.UIMenuEntry> entries = new ();

            //ui.uimenu_uicontainer.AddEntry("Action", () => { subState = MenuSubState.Action; PrepareSpellMenu(); }, "Show all avilable actions");
            //bool first = true;
            int currentKey = 1;
            foreach (var action in currentUnit.data.availableActions)
            {
                if (action.data.isActivelyUsed.Eval(currentUnit.data))
                {
                    //AddActionEntry(action, first);
                    //first = false;
                    entries.Add(miniRAID.UIElements.UnitMenuController.GetActionEntry(action, currentUnit, $"{currentKey}", null, AutoPass(currentUnit)));
                    currentKey++;
                }
            }

            //ui.uimenu_uicontainer.AddEntry("*DEBUG", () => { Globals.debugMessage.Instance.Message("Test"); });
            //ui.uimenu_uicontainer.AddEntry("Pass", OnPassSelected());
            entries.Add(new miniRAID.UIElements.UnitMenuController.UIMenuEntry
            {
                text = "Pass",
                action = OnPassSelected(),
                onFinished = null,
                toolTip = "",
                keycode = "R"
            });

            ui.combatView.menu.PrepareMenu(entries);
        }

        private void PrepareSpellMenu()
        {
            ui.uimenu_uicontainer.Clear();
            bool first = true;
            foreach (var action in currentUnit.data.availableActions)
            {
                if(action.data.isActivelyUsed.Eval(currentUnit.data))
                {
                    AddActionEntry(action, first);
                    first = false;
                }
            }
        }

        private void AddActionEntry(RuntimeAction action, bool isFirst)
        {
            ui.uimenu_uicontainer.AddActionEntry(
                action, currentUnit,
                null, isFirst, AutoPass(currentUnit));

            //ui.uimenu_uicontainer.AddActionEntry(
            //    action, currentUnit,
            //    // Before Animation
            //    null, //WaitForAnimation,
            //    // After Animation
            //    () => {
            //        //AnimationFinish();
            //        PrepareSpellMenu();
            //        ui.ShowMainMobStats(currentUnit);
            //        if (currentUnit.isActive == false)
            //        {
            //            OnPassSelected();
            //        }
            //    },
            //    null, isFirst
            //);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            ui.uimenu_uicontainer.Hide();
        }

        public IEnumerator OnPassSelected(bool set = true)
        {
            ui.EnterState();

            if (set)
            {
                yield return new JumpIn(currentUnit.data.SetActive(false));
            }
        }

        public IEnumerator AutoPass(MobRenderer mobRenderer)
        {
            keepCameraPosition = true;

            yield return new JumpIn(mobRenderer.data.TryAutoEndTurn());

            if(mobRenderer.data.isActive == false)
            {
                yield return new JumpIn(OnPassSelected(false));
            }
        }
    }
}
