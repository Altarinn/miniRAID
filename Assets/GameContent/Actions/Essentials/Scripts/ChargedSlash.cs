using System.Collections;
using System.Collections.Generic;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using miniRAID.Weapon;
using UnityEngine;

namespace miniRAID.Actions
{
    public class ChargedSlash : ChargedActionSO
    {
        [SerializeField] private GridShape shape;
        [SerializeField] private UnitFilters filter;
        [SerializeField] private SpellDamageHeal damage;
        [SerializeField] private SimpleExplosionFx fx;

        public override Dictionary<string, object> LazyPrepareTooltipVariables(RuntimeAction ract)
        {
            var vars = base.LazyPrepareTooltipVariables(ract);
            vars.Add("HitPower", damage.GetPower(ract));

            return vars;
        }

        public override IEnumerator OnPerformChargedAttack(RuntimeAction ract, MobData mob, SpellTarget target)
        {
            shape.position = target.targetPos[0];
            var mainTargetPos = target.targetPos[0];

            shape.direction = Globals.backend.GetDominantDirection(mob.Position, mainTargetPos);
            var capturedTargets = CaptureTargetsInGridShape.CaptureAllTargetsWithinRange(
                mob, filter, shape.ApplyTransform());
            
            yield return new JumpIn(fx.Do(mob.Position + 2 * GridShape.directionVectors[(int)shape.direction]));

            foreach (var targetMob in capturedTargets)
            {
                yield return new JumpIn(damage.Do(ract, mob, targetMob));
            }
        }
    }
}