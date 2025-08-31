using System.Collections;
using miniRAID.ActionHelpers;
using miniRAID.Agents;
using miniRAID.Backend;
using miniRAID.Spells;
using miniRAID.TurnSchedule;
using Sirenix.Serialization;

namespace miniRAID.MobBehaviour.TurnSlices
{
    public class SimpleUseSingleMobActionTurnSliceSO : SimpleUseTargetedActionTurnSliceSO
    {
        public override MobActionTurnSlice Wrap(MobData mob, RuntimeAction action, TurnSliceMetadata metadata)
        {
            return new SimpleUseSingleMobActionTurnSlice(mob, action, this, metadata);
        }

        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SimpleUseSingleMobActionTurnSlice : SimpleUseTargetedActionTurnSlice<SingleMobTarget>, IRenderableState
    {
        private bool IgnoreWall => action.data.IgnoreWall;

        public SimpleUseSingleMobActionTurnSlice(MobData mob, RuntimeAction action, AbstractTurnSliceSO data,
            TurnSliceMetadata metadata) : base(mob, action, data, metadata) { }

        protected override SingleMobTarget GetTarget(MobData self)
        {
            var targetIndicator = self.FindListener<TargetIndicator>();
            
            if (targetIndicator == null) return null;
            if (targetIndicator.CurrentTarget == null) return null;
            
            if (action is RuntimeAction<SingleMobTarget> ra)
                return ValidTargetFinder.FindValidSingleMobTarget(self, targetIndicator.CurrentTarget, ra);
            
            return null;
        }

        public override void ConstructRenderer()
        {
            if (target == null) return;
            
            GridCollider indicatorShape = (GridCollider)((RuntimeAction<SingleMobTarget>)action)?.Shape?.CloneWithNewGuid();
            indicatorShape.Position = target.TargetPosition;
            indicatorShape.Direction = Consts.Direction.Up;
            
            if (indicatorShape != null)
            {
                renderer = new GridColliderIndicator(
                    indicatorShape, GridOverlay.Types.INCOMING_ATTACK);
                UpdateRenderer();
            }
        }

        public override void UpdateRenderer()
        {
            if(target == null) return;
            
            GridCollider indicatorShape = (renderer as GridColliderIndicator)?.collider;
            if (indicatorShape != null)
            {
                indicatorShape.Position = target.TargetPosition;
                indicatorShape.Direction = Consts.Direction.Up;
                (renderer as GridColliderIndicator)?.Update(indicatorShape, null);
            }
        }
    }
}