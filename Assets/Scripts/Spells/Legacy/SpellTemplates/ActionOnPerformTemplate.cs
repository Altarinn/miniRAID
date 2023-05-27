using miniRAID.Spells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID
{
    public abstract class ActionOnPerformTemplate : LuaGetterTemplate<(GeneralCombatData, Mob, Spells.SpellTarget), IEnumerator>
    {
        public override IEnumerator Eval((GeneralCombatData, Mob, SpellTarget) param)
        {
            GeneralCombatData self = param.Item1;
            Mob mob = param.Item2;
            Spells.SpellTarget target = param.Item3;
            yield return new JumpIn(EasyEval(self, mob, target));
        }

        public abstract IEnumerator EasyEval(GeneralCombatData self, Mob mob, SpellTarget target);
    }
}
