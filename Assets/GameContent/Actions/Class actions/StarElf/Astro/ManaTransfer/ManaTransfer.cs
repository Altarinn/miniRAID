using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Spells;

namespace miniRAID
{
    public class ManaTransfer : ActionDataSO
    {
        public LeveledStats<float> transferRate;

        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget targetPos)
        {
            var target = Globals.backend.GetMap(targetPos.targetPos[0])?.mob;

            if (!costs.ContainsKey(Cost.Type.Mana))
            {
                yield break;
            }

            float manaAmount = (float)costs[Cost.Type.Mana].Eval((mob, targetPos));
            var tgtMana = target?.FindListener<GeneralManaListener>();

            if (manaAmount <= 0 || tgtMana == null)
            {
                yield break;
            }
            
            tgtMana.AddMana(Mathf.CeilToInt(manaAmount * transferRate.Eval(ract.level)));
        }
    }
}
