using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace miniRAID.UI
{
    [RequireComponent(typeof(Button))]
    public class ActionButtonModifier : MonoBehaviour
    {
        public RuntimeAction action;
        public MobRenderer source;
        public string nameOverride;

        Button button;
        TMPro.TextMeshProUGUI mainText;

        // Use this for initialization
        void Start()
        {
            mainText = transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            button = GetComponent<Button>();
            RefreshState();
        }

        public void SetData(RuntimeAction action, MobRenderer source, string nameOverride)
        {
            this.action = action;
            this.source = source;
            this.nameOverride = nameOverride;
        }

        public void RefreshState()
        {
            string name = nameOverride;
            if (name == null) { name = action.data.ActionName; }

            if (source.data.CheckCalculatedActionCostBounds(action))
            {
                mainText.text = name;
                button.interactable = true;
            }
            else
            {
                if(action.cooldownRemain > 0)
                {
                    mainText.text = $"CD ({action.cooldownRemain}) {name}";
                }
                else
                {
                    mainText.text = $"{name}";
                }
                button.interactable = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //RefreshState();
        }
    }
}
