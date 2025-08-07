using System.Collections;
using System.Collections.Generic;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using miniRAID.Weapon;
using UnityEngine;

namespace miniRAID.Actions
{
    public class ChargedSlash : ChargedActionSO<FourDirectionalTarget>
    {
        [SerializeField] private IGridCollider shape;
        [SerializeField] private UnitFilters filter;
        [SerializeField] private SpellDamageHeal damageOrHeal;
        [SerializeField] private SimpleExplosionFx fx;

        public override Dictionary<string, object> LazyPrepareTooltipVariables(RuntimeAction ract)
        {
            var vars = base.LazyPrepareTooltipVariables(ract);
            vars.Add("HitPower", damageOrHeal.GetPower(ract));

            return vars;
        }

        public override IEnumerator OnPerformChargedAttack(
            ChargedAction<FourDirectionalTarget> ract, MobData mob, FourDirectionalTarget target)
        {
            shape.Position = mob.Position;
            shape.Direction = target.Target;
            
            var capturedTargets = CaptureTargetsInCollider.CaptureAllTargetsWithinRange(
                mob, filter, shape);
            
            yield return new JumpIn(fx.Do(mob.Position + 2 * Consts.DirectionVectors[(int)shape.Direction]));

            yield return new JumpIn(
                MobListHelpers.WaitForAllMobs(capturedTargets, m => damageOrHeal.Do(ract, mob, m))
            );
        }
    }
}