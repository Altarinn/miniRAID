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

        miniRAID.UI.GridUI ui;

        Dictionary<string, System.Action> shortcutList;

        public struct UIMenuEntry
        {
            public string text;
            public IEnumerator action;
            public IEnumerator onFinished;
            public string toolTip;

            public string keycode;

            public RuntimeAction runtimeAction;
            public Mob source;
        }

        public class ButtonData
        {
            public System.Action currentAction;
        }

        public UnitMenuController(VisualElement elem)
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

            System.Action act = () => { ui.WaitFor(entry.action, entry.onFinished); };

            if (((ButtonData)btn.userData).currentAction != null)
            {
                btn.clicked -= ((ButtonData)btn.userData).currentAction;
            }

            btn.clicked += act;
            ((ButtonData)btn.userData).currentAction = act;

            btn.RegisterCallback<PointerEnterEvent>(
                evt =>
                {
                    toolTipContainer.style.visibility = Visibility.Visible;
                    toolTip.text = entry.toolTip;
                }
            );
            btn.RegisterCallback<PointerLeaveEvent>(
                evt =>
                {
                    toolTipContainer.style.visibility = Visibility.Hidden;
                }
            );

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
                e.Q<Label>("APCost").text = new StringBuilder().Insert(0, "✧", apcostMin).ToString();
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
                if (!entry.source.CheckCalculatedActionCostBounds(entry.runtimeAction))
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

            Debug.Log(entries.Count);
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
            RuntimeAction action, Mob source, string keycode = null,
            string nameOverride = null, IEnumerator onFinished = null)
        {
            return new UIMenuEntry
            {
                text = action.data.ActionName,
                action = action.RequestInUI(source),
                onFinished = onFinished,
                toolTip = action.GetTooltip(source),
                source = source,
                runtimeAction = action,
                keycode = keycode,
            };
        }
    }
}
