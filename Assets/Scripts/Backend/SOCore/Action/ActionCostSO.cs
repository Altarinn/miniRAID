using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace miniRAID
{
    [CreateAssetMenu(menuName = "Action/ActionCost")]
    public class ActionCostSO : ScriptableObject
    {
        ActionDataSO action;

        public int ManaCost;
        public int ActionPointCost;
        public int Cooldown;
        public GCDGroup gcdGroup = GCDGroup.Common;

        private void Awake()
        {
            Debug.Log("Test");
        }

        public virtual bool CheckCosts(MobRenderer mobRenderer, RuntimeAction ract)
        {
            // TODO
            return true;
                //mob.data.availableActions.Contains(this) &&
                //!mob.data.IsInGCD(this.GCDgroup) &&
                //CooldownRemain <= 0;
        }

        public void DoCosts(MobRenderer mobRenderer)
        {
            // TODO
        }
    }
}
