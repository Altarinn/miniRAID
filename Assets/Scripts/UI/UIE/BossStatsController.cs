using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;

namespace miniRAID.UIElements
{
    public class BossStatsController
    {
        VisualElement HPBar;
        Label nameLevelLabel, buffList, incomingList, debuffList, hpNumber, hpPercentage;

        VisualElement masterElem;

        MobRenderer _mobRenderer;

        miniRAID.Agents.MobAgentBase agent;

        public BossStatsController(VisualElement e)
        {
            masterElem = e;

            HPBar = e.Q<VisualElement>("HPContent");

            nameLevelLabel = e.Q<Label>("Name");
            buffList = e.Q<Label>("Buffs");
            incomingList = e.Q<Label>("Incoming");
            debuffList = e.Q<Label>("Debuffs");
            hpNumber = e.Q<Label>("HPNum");
            hpPercentage = e.Q<Label>("HPPercentage");
        }

        public void Register(MobRenderer mobRenderer)
        {
            this._mobRenderer = mobRenderer;
            this.agent = (miniRAID.Agents.MobAgentBase)mobRenderer.data.listeners.Find(x => x is miniRAID.Agents.MobAgentBase);
            Update();
        }

        public void Update()
        {
            if(this._mobRenderer == null)
            {
                nameLevelLabel.text = "UNKNOWN";
            }
            else
            {
                nameLevelLabel.text = $"Lv.{_mobRenderer.data.level} {_mobRenderer.data.nickname}";
                hpNumber.text = $"{_mobRenderer.data.health} / {_mobRenderer.data.maxHealth}";
                hpPercentage.text = $"{_mobRenderer.data.health / (float)_mobRenderer.data.maxHealth * 100.0f:0.0}%";

                HPBar.style.width = new StyleLength(new Length((float)_mobRenderer.data.health / (float)_mobRenderer.data.maxHealth * 100.0f, LengthUnit.Percent));

                string effects = "";
                foreach (var fx in _mobRenderer.data.listeners)
                {
                    if (fx.type == MobListenerSO.ListenerType.Buff)
                    {
                        if(effects != "")
                        {
                            effects += " " + fx.name;
                        }
                        else
                        {
                            effects += fx.name;
                        }
                    }
                }

                buffList.text = $"BUFF LIST\n{effects}";
                debuffList.text = $"DEBUFF LIST\nNOT IMPLEMENTED";

                if (this.agent == null)
                {
                    incomingList.text = $"- INCOMING -\nNO AGENT";
                }
                else
                {
                    incomingList.text = $"- INCOMING -\n{agent.GetIncomingString(_mobRenderer.data)}";
                }
            }
        }
    }
}
