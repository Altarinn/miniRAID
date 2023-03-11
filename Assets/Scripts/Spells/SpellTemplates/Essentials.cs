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
    public class CaptureAllTargetsWithinRange : LuaGetterTemplate<(GeneralCombatData, Mob, Vector2Int), IEnumerator>
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
        public LuaFunc<(GeneralCombatData, Mob, Mob), IEnumerator> ForEach;

        public override IEnumerator Eval((GeneralCombatData, Mob, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            Mob src = param.Item2;
            Vector2Int center = param.Item3;

            GridShape range = getRange.Eval((center, center + (center - src.data.Position)));
            if(range == null)
            {
                range = self.shape;
            }

            HashSet<Mob> captured = new HashSet<Mob>();

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
                            if (Consts.ApplyMask(Consts.AllyMask(src.data.unitGroup), grid.mob.data.unitGroup))
                            {
                                continue;
                            }
                        }

                        if (!captureEnemies)
                        {
                            if (Consts.ApplyMask(Consts.EnemyMask(src.data.unitGroup), grid.mob.data.unitGroup))
                            {
                                continue;
                            }
                        }

                        Mob dst = grid.mob;
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
                    foreach (Mob dst in captured)
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
                    (Databackend.IsMobValidFunc)((Mob mob) => Consts.ApplyMask(mask, mob.data.unitGroup)),
                    (Databackend.IsGridValidFunc)((Vector2Int pos, GridData grid) => ((Consts.Distance(center, pos) <= ballRadius)))
                );
                captured = capturedGrids.Select(p => Globals.backend.getMap(p.x, p.y).mob).ToHashSet();

                foreach (Mob dst in captured)
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
        public LuaFunc<(GeneralCombatData, Mob, Vector2Int), IEnumerator> ForEach;

        public override IEnumerator EasyEval(GeneralCombatData self, Mob mob, SpellTarget target)
        {
            foreach(var pos in target.targetPos)
            {
                yield return new JumpIn(ForEach.Eval((self, mob, pos)));
            }
        }
    }

    public class AtSelf : ActionOnPerformTemplate
    {
        [EventSlot]
        public LuaFunc<(GeneralCombatData, Mob, Vector2Int), IEnumerator> Action;

        public override IEnumerator EasyEval(GeneralCombatData self, Mob mob, SpellTarget target)
        {
            yield return new JumpIn(Action.Eval((self, mob, mob.data.Position)));
        }
    }

    public class AtNthTarget : ActionOnPerformTemplate
    {
        public int index = 0;

        [EventSlot]
        public LuaFunc<(GeneralCombatData, Mob, Vector2Int), IEnumerator> Action;

        public override IEnumerator EasyEval(GeneralCombatData self, Mob mob, SpellTarget target)
        {
            yield return new JumpIn(Action.Eval((
                self, 
                mob, 
                target.targetPos[Mathf.Min(target.targetPos.Count - 1, index)]
            )));
        }
    }

    public class ToSpellTarget : LuaGetterTemplate<(GeneralCombatData, Mob, Mob), IEnumerator>
    {
        [EventSlot]
        public LuaFunc<(GeneralCombatData, Mob, SpellTarget), IEnumerator> Action;

        public override IEnumerator Eval((GeneralCombatData, Mob, Mob) param)
        {
            yield return new JumpIn(Action.Eval((param.Item1, param.Item2, new SpellTarget(param.Item3.data.Position))));
        }
    }

    public class ToVector2Int : LuaGetterTemplate<(GeneralCombatData, Mob, Mob), IEnumerator>
    {
        [EventSlot]
        public LuaFunc<(GeneralCombatData, Mob, Vector2Int), IEnumerator> Action;

        public override IEnumerator Eval((GeneralCombatData, Mob, Mob) param)
        {
            yield return new JumpIn(Action.Eval((param.Item1, param.Item2, param.Item3.data.Position)));
        }
    }

    public class DoDamage : LuaGetterTemplate<(GeneralCombatData, Mob, Mob), IEnumerator>
    {
        public LuaGetter<Mob, Consts.Elements> damageType;

        // TODO: Default value
        [LabelText("Power%")]
        public LuaGetter<Mob, float> power = new LuaGetter<Mob, float>(1.0f);

        public override IEnumerator Eval((GeneralCombatData, Mob, Mob) param)
        {
            GeneralCombatData self = param.Item1;
            Mob src = param.Item2;
            Mob dst = param.Item3;

            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine.");
            yield return new JumpIn(Globals.backend.DealDmgHeal(dst,
                new Consts.DamageHeal_FrontEndInput
                {
                    source = src,
                    value = power.Eval(src) * self.power,
                    type = damageType.Eval(src),

                    crit = src.data.crit,

                    sourceAction = self.ract,
                }
            ));

            yield break;
        }
    }

    public class ApplyBuff : LuaGetterTemplate<(GeneralCombatData, Mob, Mob), IEnumerator>
    {
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public Buff.BuffSO buff;

        [LabelText("Power%")]
        public float power;

        [LabelText("Aux. power%")]
        public float auxPower;

        public override IEnumerator Eval((GeneralCombatData, Mob, Mob) param)
        {
            GeneralCombatData self = param.Item1;
            Mob src = param.Item2;
            Mob dst = param.Item3;

            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine.");
            Buff.Buff rbuff = (Buff.Buff)buff.Wrap(src.data);
            rbuff.power = dNumber.CreateComposite(self.power.Value * power);
            rbuff.auxPower = dNumber.CreateComposite(self.auxPower * auxPower);
            rbuff.crit = dNumber.CreateComposite(src.data.crit);
            dst.ReceiveBuff(rbuff);

            yield break;
        }
    }

    public class DoDamageAtGrid : LuaGetterTemplate<(GeneralCombatData, Mob, Vector2Int), IEnumerator>
    {
        public LuaGetter<Mob, Consts.Elements> damageType;

        // TODO: Default value
        [LabelText("Power%")]
        public LuaGetter<Mob, float> power = new LuaGetter<Mob, float>(1.0f);

        public override IEnumerator Eval((GeneralCombatData, Mob, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            Mob src = param.Item2;

            GridData grid = Globals.backend.getMap(param.Item3.x, param.Item3.y);
            if(grid == null) { yield break; }

            Mob dst = grid.mob;
            if(dst == null) { yield break; }

            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine.");
            yield return new JumpIn(Globals.backend.DealDmgHeal(dst,
                new Consts.DamageHeal_FrontEndInput
                {
                    source = src,
                    value = power.Eval(src) * self.power,
                    type = damageType.Eval(src),

                    crit = src.data.crit,

                    sourceAction = self.ract,
                }
            ));

            yield break;
        }
    }

    public class BuffAtGrid : LuaGetterTemplate<(GeneralCombatData, Mob, Vector2Int), IEnumerator>
    {
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public Buff.BuffSO buff;

        [LabelText("Power%")]
        public float power;

        [LabelText("Aux. power%")]
        public float auxPower;

        public override IEnumerator Eval((GeneralCombatData, Mob, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            Mob src = param.Item2;

            GridData grid = Globals.backend.getMap(param.Item3.x, param.Item3.y);
            if (grid == null) { yield break; }

            Mob dst = grid.mob;
            if (dst == null) { yield break; }

            // TODO: Change to coroutine.
            Debug.LogError("Refactor this to coroutine.");
            Buff.Buff rbuff = (Buff.Buff)buff.Wrap(src.data);
            rbuff.power = dNumber.CreateComposite(self.power * power);
            rbuff.auxPower = dNumber.CreateComposite(self.auxPower * auxPower);
            rbuff.crit = dNumber.CreateComposite(src.data.crit);
            dst.ReceiveBuff(rbuff);

            yield break;
        }
    }

    public class SummonAtGrid : LuaGetterTemplate<(GeneralCombatData, Mob, Vector2Int), IEnumerator>
    {
        public Mob mobPrefab;

        [EventSlot]
        public LuaFunc<(GeneralCombatData, Mob, Mob), IEnumerator> Initialization;

        public override IEnumerator Eval((GeneralCombatData, Mob, Vector2Int) param)
        {
            GeneralCombatData self = param.Item1;
            Mob src = param.Item2;

            var position = Globals.backend.FindNearestEmptyGrid(param.Item3);

            var summoned = GameObject.Instantiate(mobPrefab.gameObject, Globals.backend.GridToWorldPos(position) + Vector2.one * 0.5f, Quaternion.identity).GetComponent<Mob>();

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
