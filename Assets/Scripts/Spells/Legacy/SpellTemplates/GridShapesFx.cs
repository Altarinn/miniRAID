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
    public class PopulateGridShapeAt : LuaGetterTemplate<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator>
    {
        // TODO: Default value

        [EventSlot]
        public LuaFunc<(Vector2, Vector2), GridShape> getRange;

        [EventSlot]
        public LuaFunc<(GeneralCombatData, MobRenderer, SpellTarget), IEnumerator> Action;

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            MobRenderer src = param.Item2;
            Vector2Int center = param.Item3;

            GridShape range = getRange.Eval((center, center + (center - src.data.Position)));
            if (range == null)
            {
                range = self.shape;
            }

            List<Vector2Int> result = range.shape.Select((x) => x + center).ToList();
            SpellTarget target = new SpellTarget(result);

            yield return new JumpIn(Action.Eval((self, src, target)));
        }
    }

    public class CreateGridEffectAt : LuaGetterTemplate<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator>
    {
        public Buff.GridEffectSO fx;

        [EventSlot]
        public LuaFunc<(Vector2, Vector2), GridShape> getRange;

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            MobRenderer src = param.Item2;
            Vector2Int center = param.Item3;

            GridShape range = getRange.Eval((center, center + (center - src.data.Position)));
            if (range == null)
            {
                range = self.shape;
            }

            List<Vector2Int> result = range.shape.Select((x) => x + center).ToList();

            Buff.GridEffect rfx = (Buff.GridEffect)fx.WrapFx(src.data, new Vector3(center.x, center.y, 0));
            result.ForEach((p) => rfx.Extend(p));

            yield break;
        }
    }
}
