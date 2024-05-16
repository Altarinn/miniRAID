using System.Collections;
using System.Collections.Generic;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using miniRAID.Weapon;
using UnityEngine;

namespace miniRAID.Agents.Test
{
    public class MeleeAoETest : ActionDataSO<FourDirectionalTarget>
    {
        [SerializeField] private GridShape shape;
        [SerializeField] private UnitFilters filter;
        [SerializeField] private SpellDamageHeal damageOrHeal;
        [SerializeField] private SimpleExplosionFx fx;
        
        public override Dictionary<string, object> LazyPrepareTooltipVariables(RuntimeAction ract)
        {
            var vars = base.LazyPrepareTooltipVariables(ract);
            vars.Add("HitPower", damageOrHeal.GetPower(ract));

            return vars;
        }

        public override IEnumerator OnPerform(RuntimeAction<FourDirectionalTarget> ract, MobData mob, FourDirectionalTarget target)
        {
            shape.position = mob.Position;
            shape.direction = target.Target;
            
            var capturedTargets = CaptureTargetsInGridShape.CaptureAllTargetsWithinRange(
                mob, filter, shape.ApplyTransform());
            
            yield return new JumpIn(fx.Do(mob.Position + 2 * Consts.DirectionVectors[(int)shape.direction]));

            foreach (var targetMob in capturedTargets)
            {
                yield return new JumpIn(damageOrHeal.Do(ract, mob, targetMob));
            }
        }
        
    }
}