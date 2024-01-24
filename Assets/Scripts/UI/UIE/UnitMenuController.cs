using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using System;
using System.Linq;

using miniRAID;
using System.Text;

namespace miniRAID.UIElements
{
    public class UnitMenuController
    {
        VisualTreeAsset unitMenuButtonTemplate;
        ListView view;
        VisualElement masterElem, toolTipContainer;
        Label toolTip;

        private MobDetailsController mobDetailsController;
        private MobInfoController mobInfoController;

        miniRAID.UI.GridUI ui;

        Dictionary<string, System.Action> shortcutList;

        public struct UIMenuEntry
        {
            public string text;
            public IEnumerator action;
            public IEnumerator onFinished;

            public bool useDefaultToolTip;
            public string toolTip;
            public System.Action onPointerEnter;
            public System.Action onPointerLeave;

            public string keycode;

            public RuntimeAction runtimeAction;
            public MobRenderer source;
        }

        public class ButtonData
        {
            public System.Action currentAction;
            public EventCallback<PointerEnterEvent> pointerEnter;
            public EventCallback<PointerLeaveEvent> pointerLeave;
        }

        public UnitMenuController(VisualElement elem, MobDetailsController mobDetailsController, MobInfoController mobInfoController)
        {
            masterElem = elem;

            unitMenuButtonTemplate = Resources.Load<VisualTreeAsset>("UI/UnitMenuEntry");
            view = elem.Q<ListView>();
            toolTip = elem.Q<Label>("ToolTipLabel");
            toolTipContainer = elem.Q("ToolTip");

            view.makeItem = () => unitMenuButtonTemplate.CloneTree();
            view.bindItem = (e, i) => BindEntryItem(e, (UIMenuEntry)view.itemsSource[i]);
            view.fixedItemHeight = 18;

            ui = Globals.ui.Instance;

            this.mobDetailsController = mobDetailsController;
            this.mobInfoController = mobInfoController;
        }

        public void BindEntryItem(VisualElement e, UIMenuEntry entry)
        {
            bool useable = true;
            e.Q<Label>("ActionName").text = entry.text;

            Button btn = e.Q<Button>();

            if (btn.userData == null)
            {
                btn.userData = new ButtonData();
            }
            
            ////////////////////////////
            // Click event
            ////////////////////////////

            System.Action act = () => { ui.WaitFor(entry.action, entry.onFinished); };

            if (((ButtonData)btn.userData).currentAction != null)
            {
                btn.clicked -= ((ButtonData)btn.userData).currentAction;
            }

            btn.clicked += act;
            ((ButtonData)btn.userData).currentAction = act;

            ////////////////////////////
            // Pointer Enter event
            ////////////////////////////
            
            if (((ButtonData)btn.userData).pointerEnter != null)
            {
                btn.UnregisterCallback(((ButtonData)btn.userData).pointerEnter);
            }

            if (entry.useDefaultToolTip)
            {
                ((ButtonData)btn.userData).pointerEnter = evt =>
                {
                    toolTipContainer.style.visibility = Visibility.Visible;
                    toolTip.text = entry.toolTip;
                };
            }
            else
            {
                ((ButtonData)btn.userData).pointerEnter = evt =>
                {
                    entry.onPointerEnter?.Invoke();
                };
            }
            
            btn.RegisterCallback(((ButtonData)btn.userData).pointerEnter);
            
            ////////////////////////////
            // Pointer Leave event
            ////////////////////////////
            
            if (((ButtonData)btn.userData).pointerLeave != null)
            {
                btn.UnregisterCallback(((ButtonData)btn.userData).pointerLeave);
            }
            
            if (entry.useDefaultToolTip)
            {
                ((ButtonData)btn.userData).pointerLeave = evt =>
                {
                    toolTipContainer.style.visibility = Visibility.Hidden;
                };
            }
            else
            {
                ((ButtonData)btn.userData).pointerLeave = evt =>
                {
                    entry.onPointerLeave?.Invoke();
                };
            }
            
            btn.RegisterCallback(((ButtonData)btn.userData).pointerLeave);

            ////////////////////////////
            // Appearance / Shortcut
            ////////////////////////////
            
            if(entry.keycode != null)
            {
                e.Q<Label>("Key").text = entry.keycode;
            }
            else
            {
                e.Q<Label>("Key").text = "";
            }

            // Find AP Cost
            if (entry.runtimeAction != null)
            {
                int apcostMin = 0;
                foreach (var costBound in entry.runtimeAction.costBounds)
                {
                    if (costBound.Item1.type == Cost.Type.AP)
                    {
                        apcostMin = costBound.Item1.value;
                        break;
                    }
                }
                e.Q<Label>("APCost").style.display = DisplayStyle.Flex;
                e.Q<Label>("APCost").text = new StringBuilder().Insert(0, "o", apcostMin).ToString();
                e.Q<Label>("Cooldown").style.display = DisplayStyle.None;
            }
            else
            {
                e.Q<Label>("APCost").style.display = DisplayStyle.None;
                e.Q<Label>("Cooldown").style.display = DisplayStyle.None;
            }

            // Update action info
            btn.SetEnabled(true);
            btn.RemoveFromClassList("disabled");

            if (entry.source != null && entry.runtimeAction != null)
            {
                if (!entry.source.data.CheckCalculatedActionCostBounds(entry.runtimeAction))
                {
                    btn.SetEnabled(false);
                    btn.AddToClassList("disabled");
                    useable = false;

                    if (entry.runtimeAction.cooldownRemain > 0)
                    {
                        e.Q<Label>("APCost").style.display = DisplayStyle.None;
                        e.Q<Label>("Cooldown").style.display = DisplayStyle.Flex;
                        e.Q<Label>("Cooldown").text = $"◷ {entry.runtimeAction.cooldownRemain}";
                    }
                }
            }

            // Add to shortcut list
            if (useable && entry.keycode != null)
            {
                shortcutList.Add(entry.keycode, act);
            }
        }

        public void ShortCut(string keyCode)
        {
            if (!IsMenuShown) { return; }
            if(shortcutList.TryGetValue(keyCode, out var act))
            {
                act.Invoke();
            }
        }

        public void PrepareMenu(List<UIMenuEntry> entries)
        {
            view.itemsSource = entries;
            shortcutList = new();

            view.RefreshItems();
            ShowMenu();

            // Debug.Log(entries.Count);
        }

        public void ShowMenu()
        {
            if (view.itemsSource.Count > 0)
            {
                masterElem.style.visibility = Visibility.Visible;
            }
        }

        public void HideMenu()
        {
            masterElem.style.visibility = Visibility.Hidden;
        }

        public bool IsMenuShown => masterElem.style.visibility == Visibility.Visible;

        public void ClearMenu()
        {
            view.itemsSource = new List<UIMenuEntry>();
            shortcutList = new();
            //view.RefreshItems();
            HideMenu();
        }

        public static UIMenuEntry GetActionEntry(
            RuntimeAction action, MobRenderer source, string keycode = null,
            string nameOverride = null, IEnumerator onFinished = null)
        {
            return new UIMenuEntry
            {
                text = action.data.ActionName,
                action = action.RequestInUI(source.data),
                onFinished = onFinished,
                useDefaultToolTip = true,
                toolTip = action.GetFullTooltip(source.data),
                source = source,
                runtimeAction = action,
                keycode = keycode,
            };
        }
        
        public UIMenuEntry GetEquipmentDetailsEntry(
            MobRenderer source, string keycode = null,
            string nameOverride = null, IEnumerator onFinished = null)
        {
            return new UIMenuEntry
            {
                text = "Equipments",
                action = null,
                onFinished = onFinished,
                useDefaultToolTip = false,
                toolTip = "",
                onPointerEnter = () => { mobDetailsController.Show(source.data); },
                onPointerLeave = () => { mobDetailsController.Hide(); },
                source = source,
                keycode = keycode,
            };
        }
        
        public UIMenuEntry GetMobDetailsEntry(
            MobRenderer source, string keycode = null,
            string nameOverride = null, IEnumerator onFinished = null)
        {
            return new UIMenuEntry
            {
                text = "Info",
                action = null,
                onFinished = onFinished,
                useDefaultToolTip = false,
                toolTip = "",
                onPointerEnter = () => { mobInfoController.Show(source.data); },
                onPointerLeave = () => { mobInfoController.Hide(); },
                source = source,
                keycode = keycode,
            };
        }
    }
}
