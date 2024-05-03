using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Spells;

namespace miniRAID
{
    public class ManaTransfer : ActionDataSO<SingleMobTarget>
    {
        public LeveledStats<float> transferRate;

        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob,
            SingleMobTarget target)
        {
            if (!costs.ContainsKey(Cost.Type.Mana))
            {
                yield break;
            }

            float manaAmount = (float)costs[Cost.Type.Mana].Eval((mob, target));
            var tgtMana = target.Target?.FindListener<GeneralManaListener>();

            if (manaAmount <= 0 || tgtMana == null)
            {
                yield break;
            }
            
            tgtMana.AddMana(Mathf.CeilToInt(manaAmount * transferRate.Eval(ract.level)));
        }
    }
}
