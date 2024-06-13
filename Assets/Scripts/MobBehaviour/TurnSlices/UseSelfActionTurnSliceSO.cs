using System.Collections;
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

    public class UseSelfActionTurnSlice : PreparableActionTurnSlice
    {
        private GridShape indicatorShape;
        
        public UseSelfActionTurnSlice(MobData mob, RuntimeAction action, AbstractTurnSliceSO data, TurnSliceMetadata metadata) : base(mob, action, data, metadata)
        {
            indicatorShape = new GridShape(((RuntimeAction<SingleMobTarget>)action).Shape);
            indicatorShape.position = mob.Position;
            
            AddIndicator(new GridShapeIndicator(
                indicatorShape, GridOverlay.Types.INCOMING_ATTACK));
        }

        public override IEnumerator Turn()
        {
            RemoveAllIndicators();
            RuntimeAction<SingleMobTarget> act = (RuntimeAction<SingleMobTarget>)action;
            yield return new JumpIn(mob.DoActionWithDefaultCosts(act, new SingleMobTarget(mob)));
        }
    }
}