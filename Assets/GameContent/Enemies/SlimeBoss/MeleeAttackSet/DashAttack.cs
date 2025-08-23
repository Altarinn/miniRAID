using System.Collections;
using System.Collections.Generic;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Agents.SlimeBoss.MeleeAttackSet
{
    public class DashAttack : ActionDataSO<FourDirectionalTarget>
    {
        [SerializeField] private GridCollider shape;
        [SerializeField] private UnitFilters filter;
        [SerializeField] private SpellDamageHeal damageOrHeal;
        [SerializeField] private SimpleExplosionFx fx;
        [SerializeField] private KnockBack knockBack;

        public override GridCollider MainShape => shape;
        public int KnockbackDistance = 1;
        public int DashDistance = 5;
        
        public override Dictionary<string, object> LazyPrepareTooltipVariables(RuntimeAction ract)
        {
            var vars = base.LazyPrepareTooltipVariables(ract);
            vars.Add("HitPower", damageOrHeal.GetPower(ract));

            return vars;
        }

        public override IEnumerator OnPerform(RuntimeAction<FourDirectionalTarget> ract, MobData mob, FourDirectionalTarget target)
        {
            shape.Position = mob.Position;
            shape.Direction = target.Target;
            
            var capturedTargets = CaptureTargetsInCollider.CaptureAllTargetsWithinRange(
                mob, filter, shape, null);
            
            yield return new JumpIn(fx.Do(mob.Position + 2 * Consts.DirectionVectors[(int)shape.Direction]));
            
            // Find target grid
            Vector3Int targetGrid = mob.GridPosition + Consts.DirectionVectors[(int)target.Target] * DashDistance;
            
            // TODO: Check target grid validity

            var existingMob = Globals.backend.GetMap(targetGrid).mob;
            if (existingMob != null)
            {
                Vector3Int newPos = Globals.backend.FindNearestEmptyGrid(targetGrid);
                yield return new JumpIn(existingMob.SetPosition(newPos));
            }

            yield return new JumpIn(mob.SetPosition(targetGrid));

            yield return new JumpIn(MobListHelpers.WaitForAllMobs(
                capturedTargets,
                m => JumpInHelper.Chain(
                    damageOrHeal?.Do(ract, mob, m),
                    knockBack?.Do(m, Consts.DirectionVectors[(int)target.Target] * KnockbackDistance))
            ));
        }
    }
}