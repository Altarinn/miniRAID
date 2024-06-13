using System.Collections;
using System.Collections.Generic;
using miniRAID.ActionHelpers;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Agents.SlimeBoss.MeleeAttackSet
{
    public class DashAttack : ActionDataSO<FourDirectionalTarget>
    {
        [SerializeField] private GridShape shape;
        [SerializeField] private UnitFilters filter;
        [SerializeField] private SpellDamageHeal damageOrHeal;
        [SerializeField] private SimpleExplosionFx fx;
        [SerializeField] private KnockBack knockBack;

        public override GridShape MainShape => shape;
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
            shape.position = mob.Position;
            shape.direction = target.Target;
            
            var capturedTargets = CaptureTargetsInGridShape.CaptureAllTargetsWithinRange(
                mob, filter, shape.ApplyTransform());
            
            yield return new JumpIn(fx.Do(mob.Position + 2 * Consts.DirectionVectors[(int)shape.direction]));
            
            // Find target grid
            Vector3Int targetGrid = mob.Position + Consts.DirectionVectors[(int)target.Target] * DashDistance;
            
            targetGrid = Vector3Int.Max(Vector3Int.Min(targetGrid, Globals.backend.MapSize), Vector3Int.zero);
            // TODO: Check target grid validity

            var existingMob = Globals.backend.GetMap(targetGrid).mob;
            if (existingMob != null)
            {
                Vector3Int newPos = Globals.backend.FindNearestEmptyGrid(targetGrid);
                yield return new JumpIn(existingMob.SetPosition(newPos));
            }

            yield return new JumpIn(mob.SetPosition(targetGrid));

            foreach (var targetMob in capturedTargets)
            {
                yield return new JumpIn(damageOrHeal.Do(ract, mob, targetMob));
                yield return new JumpIn(knockBack.Do(targetMob,
                    Consts.DirectionVectors[(int)target.Target] * KnockbackDistance));
            }
        }
    }
}