using miniRAID.Spells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID
{
    public abstract class ActionOnPerformTemplate : LuaGetterTemplate<(GeneralCombatData, MobRenderer, Spells.SpellTarget), IEnumerator>
    {
        public override IEnumerator Eval((GeneralCombatData, MobRenderer, SpellTarget) param)
        {
            GeneralCombatData self = param.Item1;
            MobRenderer mobRenderer = param.Item2;
            Spells.SpellTarget target = param.Item3;
            yield return new JumpIn(EasyEval(self, mobRenderer, target));
        }

        public abstract IEnumerator EasyEval(GeneralCombatData self, MobRenderer mobRenderer, SpellTarget target);
    }
}
