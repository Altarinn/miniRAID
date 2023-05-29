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
    public class CaptureAllTargetsWithinRange : LuaGetterTemplate<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator>
    {
        // TODO: Default value
        public enum RangeType
        {
            GridShape,
            Ball
        }

        [Title("Range")]
        public RangeType rangeType;
        public GridShape rangeOverride = null;
        public int ballRadius = 0;

        [EventSlot]
        public LuaFunc<(Vector2, Vector2), GridShape> getRange;

        [Title("Target identification")]
        public bool captureAllies = false;
        public bool captureEnemies = true;
        public bool captureUnique = true;

        [EventSlot]
        public LuaFunc<(GeneralCombatData, MobRenderer, MobRenderer), IEnumerator> ForEach;

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            MobRenderer src = param.Item2;
            Vector2Int center = param.Item3;

            GridShape range = getRange.Eval((center, center + (center - src.data.Position)));
            if(range == null)
            {
                range = self.shape;
            }

            HashSet<MobRenderer> captured = new HashSet<MobRenderer>();

            if(rangeType == RangeType.GridShape)
            {
                foreach (var offset in range.shape)
                {
                    Vector2Int pos = center + offset;
                    GridData grid = Globals.backend.getMap(pos.x, pos.y);
                    if (grid != null && grid.mob != null)
                    {
                        if (!captureAllies)
                        {
                            if (Consts.ApplyMask(Consts.AllyMask(src.data.unitGroup), grid.mob.unitGroup))
                            {
                                continue;
                            }
                        }

                        if (!captureEnemies)
                        {
                            if (Consts.ApplyMask(Consts.EnemyMask(src.data.unitGroup), grid.mob.unitGroup))
                            {
                                continue;
                            }
                        }

                        MobRenderer dst = grid.mob.mobRenderer;
                        if (captureUnique)
                        {
                            if (!captured.Contains(dst))
                            {
                                captured.Add(dst);
                            }
                        }
                        else
                        {
                            yield return new JumpIn(ForEach.Eval((self, src, dst)));
                        }
                    }
                }

                if (captureUnique)
                {
                    foreach (MobRenderer dst in captured)
                    {
                        yield return new JumpIn(ForEach.Eval((self, src, dst)));
                    }
                }
            }
            else if(rangeType == RangeType.Ball)
            {
                int mask = 0;
                if (captureAllies) { mask |= Consts.AllyMask(src.data.unitGroup); }
                if (captureEnemies) { mask |= Consts.EnemyMask(src.data.unitGroup); }

                var capturedGrids = Globals.backend.GetGridsWithMob(
                    (Databackend.IsMobValidFunc)((MobData mob) => Consts.ApplyMask(mask, mob.unitGroup)),
                    (Databackend.IsGridValidFunc)((Vector2Int pos, GridData grid) => ((Consts.Distance(center, pos) <= ballRadius)))
                );
                captured = capturedGrids.Select(p => Globals.backend.getMap(p.x, p.y).mob.mobRenderer).ToHashSet();

                foreach (MobRenderer dst in captured)
                {
                    yield return new JumpIn(ForEach.Eval((self, src, dst)));
                }
            }

            yield break;
        }
    }

    // TODO: FIXME: Very weird usage right now but let it go
    // When no rotation is considered, should store GridShape (HashSet<Vector2Int>) and use that directly instead of doing collision test everytime. Waste of computation.
    public class PopulateGridShapeMono : LuaGetterTemplate<(Vector2, Vector2), GridShape>
    {
        public GridShapeMono shape;
        public bool alignRotation = false;

        public override GridShape Eval((Vector2, Vector2) param)
        {
            var origin = param.Item1;
            var lookat = param.Item2;

            if(alignRotation)
            {
                Debug.LogError("alignRotation not implemented.");
                return null;
            }
            else
            {
                var populatedShape = GameObject.Instantiate(shape.gameObject, Vector3.zero, Quaternion.identity).GetComponent<GridShapeMono>();
                GameObject.Destroy(populatedShape.gameObject, 1.0f);
                return populatedShape.Shape;
            }
        }
    }

    public class ForAllPositions : ActionOnPerformTemplate
    {
        [EventSlot]
        public LuaFunc<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator> ForEach;

        public override IEnumerator EasyEval(GeneralCombatData self, MobRenderer mobRenderer, SpellTarget target)
        {
            foreach(var pos in target.targetPos)
            {
                yield return new JumpIn(ForEach.Eval((self, mobRenderer, pos)));
            }
        }
    }

    public class AtSelf : ActionOnPerformTemplate
    {
        [EventSlot]
        public LuaFunc<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator> Action;

        public override IEnumerator EasyEval(GeneralCombatData self, MobRenderer mobRenderer, SpellTarget target)
        {
            yield return new JumpIn(Action.Eval((self, mobRenderer, mobRenderer.data.Position)));
        }
    }

    public class AtNthTarget : ActionOnPerformTemplate
    {
        public int index = 0;

        [EventSlot]
        public LuaFunc<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator> Action;

        public override IEnumerator EasyEval(GeneralCombatData self, MobRenderer mobRenderer, SpellTarget target)
        {
            yield return new JumpIn(Action.Eval((
                self, 
                mobRenderer, 
                target.targetPos[Mathf.Min(target.targetPos.Count - 1, index)]
            )));
        }
    }

    public class ToSpellTarget : LuaGetterTemplate<(GeneralCombatData, MobRenderer, MobRenderer), IEnumerator>
    {
        [EventSlot]
        public LuaFunc<(GeneralCombatData, MobRenderer, SpellTarget), IEnumerator> Action;

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, MobRenderer) param)
        {
            yield return new JumpIn(Action.Eval((param.Item1, param.Item2, new SpellTarget(param.Item3.data.Position))));
        }
    }

    public class ToVector2Int : LuaGetterTemplate<(GeneralCombatData, MobRenderer, MobRenderer), IEnumerator>
    {
        [EventSlot]
        public LuaFunc<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator> Action;

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, MobRenderer) param)
        {
            yield return new JumpIn(Action.Eval((param.Item1, param.Item2, param.Item3.data.Position)));
        }
    }

    public class DoDamage : LuaGetterTemplate<(GeneralCombatData, MobRenderer, MobRenderer), IEnumerator>
    {
        public LuaGetter<MobRenderer, Consts.Elements> damageType;

        // TODO: Default value
        [LabelText("Power%")]
        public LuaGetter<MobRenderer, float> power = new LuaGetter<MobRenderer, float>(1.0f);

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, MobRenderer) param)
        {
            GeneralCombatData self = param.Item1;
            MobRenderer src = param.Item2;
            MobRenderer dst = param.Item3;

            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine.");
            yield return new JumpIn(Globals.backend.DealDmgHeal(dst.data,
                new Consts.DamageHeal_FrontEndInput
                {
                    source = src.data,
                    value = power.Eval(src) * self.power,
                    type = damageType.Eval(src),

                    crit = src.data.crit,

                    sourceAction = self.ract,
                }
            ));

            yield break;
        }
    }

    public class ApplyBuff : LuaGetterTemplate<(GeneralCombatData, MobRenderer, MobRenderer), IEnumerator>
    {
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public Buff.BuffSO buff;

        [LabelText("Power%")]
        public float power;

        [LabelText("Aux. power%")]
        public float auxPower;

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, MobRenderer) param)
        {
            GeneralCombatData self = param.Item1;
            MobRenderer src = param.Item2;
            MobRenderer dst = param.Item3;

            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine.");
            Buff.Buff rbuff = (Buff.Buff)buff.Wrap(src.data);
            rbuff.power = dNumber.CreateComposite(self.power.Value * power);
            rbuff.auxPower = dNumber.CreateComposite(self.auxPower * auxPower);
            rbuff.crit = dNumber.CreateComposite(src.data.crit);
            dst.data.AddBuff(rbuff);

            yield break;
        }
    }

    public class DoDamageAtGrid : LuaGetterTemplate<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator>
    {
        public LuaGetter<MobRenderer, Consts.Elements> damageType;

        // TODO: Default value
        [LabelText("Power%")]
        public LuaGetter<MobRenderer, float> power = new LuaGetter<MobRenderer, float>(1.0f);

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            MobRenderer src = param.Item2;

            GridData grid = Globals.backend.getMap(param.Item3.x, param.Item3.y);
            if(grid == null) { yield break; }

            MobRenderer dst = grid.mob.mobRenderer;
            if(dst == null) { yield break; }

            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine.");
            yield return new JumpIn(Globals.backend.DealDmgHeal(dst.data,
                new Consts.DamageHeal_FrontEndInput
                {
                    source = src.data,
                    value = power.Eval(src) * self.power,
                    type = damageType.Eval(src),

                    crit = src.data.crit,

                    sourceAction = self.ract,
                }
            ));

            yield break;
        }
    }

    public class BuffAtGrid : LuaGetterTemplate<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator>
    {
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public Buff.BuffSO buff;

        [LabelText("Power%")]
        public float power;

        [LabelText("Aux. power%")]
        public float auxPower;

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            MobRenderer src = param.Item2;

            GridData grid = Globals.backend.getMap(param.Item3.x, param.Item3.y);
            if (grid == null) { yield break; }

            MobRenderer dst = grid.mob.mobRenderer;
            if (dst == null) { yield break; }

            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine.");
            Buff.Buff rbuff = (Buff.Buff)buff.Wrap(src.data);
            rbuff.power = dNumber.CreateComposite(self.power * power);
            rbuff.auxPower = dNumber.CreateComposite(self.auxPower * auxPower);
            rbuff.crit = dNumber.CreateComposite(src.data.crit);
            dst.data.AddBuff(rbuff);

            yield break;
        }
    }

    public class SummonAtGrid : LuaGetterTemplate<(GeneralCombatData, MobRenderer, Vector2Int), IEnumerator>
    {
        public MobRenderer MobRendererPrefab;

        [EventSlot]
        public LuaFunc<(GeneralCombatData, MobRenderer, MobRenderer), IEnumerator> Initialization;

        public override IEnumerator Eval((GeneralCombatData, MobRenderer, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            MobRenderer src = param.Item2;

            var position = Globals.backend.FindNearestEmptyGrid(param.Item3);

            var summoned = GameObject.Instantiate(MobRendererPrefab.gameObject, Globals.backend.GridToWorldPos(position) + Vector2.one * 0.5f, Quaternion.identity).GetComponent<MobRenderer>();

            yield return new JumpIn(Initialization.Eval((self, src, summoned)));
        }
    }

    public class Sequence<TIn> : LuaGetterTemplate<TIn, IEnumerator>
    {
        [HideLabel]
        public List<LuaFunc<TIn, IEnumerator>> sequence = new();

        public override IEnumerator Eval(TIn param)
        {
            foreach (var s in sequence)
            {
                yield return new JumpIn(s.Eval(param));
            }
        }
    }
}
