using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using XLua;

using System.Linq;

using Sirenix.OdinInspector;
using miniRAID.Spells;

namespace miniRAID
{
    public class SetAdditionalParameters : ActionOnPerformTemplate
    {
        public GameObject[] gameObjects;
        public Sprite[] sprites;

        [EventSlot]
        public LuaFunc<(GeneralCombatData, Mob, Spells.SpellTarget), IEnumerator> Action;

        public override IEnumerator EasyEval(GeneralCombatData self, Mob mob, SpellTarget target)
        {
            self.sprites = sprites;
            self.gameObjects = gameObjects;

            yield return new JumpIn(Action.Eval((
                self,
                mob,
                target
            )));
        }
    }
}