using System.Collections;
using System.Collections.Generic;
using miniRAID;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Actions
{
    public class SimpleDirectionalAoE : ActionDataSO<FourDirectionalTarget>
    {
        [SerializeField] private GridCollider shape;
        [SerializeField] private bool ignoreWalls;
        [SerializeField] private UnitFilters filter;
        [SerializeField] private SpellDamageHeal damageOrHeal;
        [SerializeField] private SpellBuff buff;
        [SerializeField] private SimpleExplosionFx fx;

        public override GridCollider MainShape => shape;

        public override Dictionary<string, object> LazyPrepareTooltipVariables(RuntimeAction ract)
        {
            var vars = base.LazyPrepareTooltipVariables(ract);
            vars.Add("HitPower", damageOrHeal.GetPower(ract));

            return vars;
        }

        public override IEnumerator OnPerform(RuntimeAction<FourDirectionalTarget> ract, MobData mob,
            FourDirectionalTarget target)
        {
            shape.Position = mob.Position;
            shape.Direction = target.Target;

            var capturedTargets =
                CaptureTargetsInCollider.CaptureAllTargetsWithinRange(mob, filter, shape,
                    IgnoreWall ? null : mob.SpellCastPivot);

            yield return new JumpIn(fx.Do(mob.Position + 2 * Consts.DirectionVectors[(int)shape.Direction]));

            yield return new JumpIn(MobListHelpers.WaitForAllMobs(
                capturedTargets,
                targetMob => JumpInHelper.Chain(
                    damageOrHeal?.Do(ract, mob, targetMob),
                    buff?.Do(ract, mob, targetMob)
                )
            ));
        }
    }
}