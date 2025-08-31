using System.Collections;
using miniRAID.Backend;
using miniRAID.Spells;
using miniRAID.TurnSchedule;

namespace miniRAID.MobBehaviour.TurnSlices
{
    public class UseSelfActionTurnSliceSO : PreparableActionTurnSliceSO
    {
        public override MobActionTurnSlice Wrap(MobData mob, RuntimeAction action, TurnSliceMetadata metadata)
        {
            return new UseSelfActionTurnSlice(mob, action, this, metadata);
        }

        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            throw new System.NotImplementedException();
        }
    }

    public class UseSelfActionTurnSlice : PreparableActionTurnSlice, IRenderableState
    {
        private GridCollider indicatorShape;
        
        public UseSelfActionTurnSlice(MobData mob, RuntimeAction action, AbstractTurnSliceSO data, TurnSliceMetadata metadata) : base(mob, action, data, metadata)
        {
            if (((RuntimeAction<SingleMobTarget>)action).Shape != null)
            {
                indicatorShape = (GridCollider)((RuntimeAction<SingleMobTarget>)action).Shape.CloneWithNewGuid();
                indicatorShape.Position = mob.Position;
            }
        }

        public override IEnumerator Turn()
        {
            DestroyRenderer();
            RuntimeAction<SingleMobTarget> act = (RuntimeAction<SingleMobTarget>)action;
            yield return new JumpIn(mob.DoActionWithDefaultCosts(act, new SingleMobTarget(mob, mob.GridPosition)));
        }

        public void ConstructRenderer()
        {
            if (indicatorShape == null) return;
            renderer = new GridColliderIndicator(
                indicatorShape, GridOverlay.Types.INCOMING_ATTACK);
        }

        public void UpdateRenderer()
        {
            // Pass
        }
    }
}