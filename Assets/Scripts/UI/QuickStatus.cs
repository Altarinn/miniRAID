using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.UI
{
    public class QuickStatus : MonoBehaviour
    {
        MobRenderer _targetMobRenderer;
        TMPro.TextMeshProUGUI text;

        // Start is called before the first frame update
        void Start()
        {
            _targetMobRenderer = GetComponentInParent<MobRenderer>();
            text = GetComponent<TMPro.TextMeshProUGUI>();

            switch (_targetMobRenderer.data.unitGroup)
            {
                case Consts.UnitGroup.Player:
                    text.color = new Color(0.8610f, 1.0f, 0.610f);
                    break;
                case Consts.UnitGroup.Enemy:
                    text.color = new Color(1.0f, 0.72f, 0.67f);
                    break;
                case Consts.UnitGroup.Ally:
                    text.color = new Color(0.58f, 0.91f, 1.0f);
                    break;
                case Consts.UnitGroup.Others:
                    text.color = new Color(1f, 1f, 1f);
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            text.text = $"{(int)((_targetMobRenderer.data.health / (float)_targetMobRenderer.data.maxHealth) * 100)}% {_targetMobRenderer.data.actionPoints}";
        }
    }
}
