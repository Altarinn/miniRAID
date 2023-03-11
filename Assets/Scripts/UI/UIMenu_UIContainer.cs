using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace miniRAID.UI
{
    public class UnitMenuItemAttribute : Attribute
    {
        public string name;
        public int priority;

        public UnitMenuItemAttribute(string name, int priority)
        {
            this.name = name;
            this.priority = priority;
        }
    }

    public class UIMenu_UIContainer : MonoBehaviour
    {
        public Transform container, toolTipContainer;
        public TMPro.TextMeshProUGUI toolTipText;
        public Button buttonPrefab;

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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        bool _visible;
        public bool Visible
        {
            get
            {
                return _visible;
            }
        }

        public void Show()
        {
            _visible = true;
            container.gameObject.SetActive(true);
        }

        public void _ShowTemp()
        {
            container.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _visible = false;
            container.gameObject.SetActive(false);
            HideToolTip();
        }

        public void _HideTemp()
        {
            container.gameObject.SetActive(false);
            HideToolTip();
        }

        public Button AddEntry(
            string text, 
            UnityEngine.Events.UnityAction action, 
            string toolTip = "",
            bool selected = false, 
            int idx = -1
        )
        {
            Button button = Instantiate(buttonPrefab.gameObject, container).GetComponent<Button>();

            if(idx > 0)
            {
                button.transform.SetSiblingIndex(idx);
            }

            if (selected)
            {
                button.Select();
            }

            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
            if (action != null) { button.onClick.AddListener(action); }
            
            var eT = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry enterEvt = new EventTrigger.Entry();
            enterEvt.eventID = EventTriggerType.PointerEnter;
            enterEvt.callback.AddListener((eventData) => {
                if (!EventSystem.current.alreadySelecting)
                    EventSystem.current.SetSelectedGameObject(eT.gameObject);
            });

            EventTrigger.Entry selectedEvt = new EventTrigger.Entry();
            selectedEvt.eventID = EventTriggerType.Select;
            selectedEvt.callback.AddListener((eventData) => {
                if (toolTip != "")
                {
                    ShowTooltip(toolTip); 
                }
            });

            EventTrigger.Entry deselectEvt = new EventTrigger.Entry();
            deselectEvt.eventID = EventTriggerType.Deselect;
            deselectEvt.callback.AddListener((eventData) => {
                eT.GetComponent<Selectable>().OnPointerExit(null);
                if (toolTip != "")
                {
                    HideToolTip();
                }
            });

            eT.triggers.Add(enterEvt);
            eT.triggers.Add(selectedEvt);
            eT.triggers.Add(deselectEvt);

            return button;
        }

        public Button AddActionEntry(
            RuntimeAction action, Mob source, 
            string nameOverride = null, bool selected = false, IEnumerator onFinished = null)
        {
            Button btn = AddEntry(
                action.data.ActionName,
                action.RequestInUI(source),
                onFinished,
                action.GetTooltip(source),
                selected
            );
            btn.gameObject.AddComponent<ActionButtonModifier>().SetData(action, source, nameOverride);

            return btn;
        }

        public Button AddEntry(
            string text,
            IEnumerator action,
            IEnumerator onFinished = null,
            string toolTip = "",
            bool selected = false,
            int idx = -1)
        {
            Button btn = AddEntry(
                text,
                () => {
                    ui.WaitFor(action, onFinished);
                },
                toolTip,
                selected,
                idx
            );

            return btn;
        }

        //public Button AddEntry(
        //    string text,
        //    IEnumerator action,
        //    string toolTip = "",
        //    bool selected = false,
        //    int idx = -1)
        //    => AddEntry(text, action, null, toolTip, selected, idx);

        public void ShowTooltip(string toolTip)
        {
            toolTipContainer.gameObject.SetActive(true);
            toolTipText.text = toolTip;
        }

        public void HideToolTip()
        {
            toolTipContainer.gameObject.SetActive(false);
        }

        public void Clear()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
