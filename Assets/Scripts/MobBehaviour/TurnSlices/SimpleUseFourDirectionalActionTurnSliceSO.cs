using System.Collections;
using miniRAID.Spells;
using miniRAID.TurnSchedule;

namespace miniRAID.MobBehaviour.TurnSlices
{
    public class SimpleUseFourDirectionalActionTurnSliceSO : MobActionTurnSliceSO
    {
        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            var mobActionSlice = (MobActionTurnSlice)slice;
            MobData mob = mobActionSlice.mob;
            RuntimeAction<FourDirectionalTarget> act = (RuntimeAction<FourDirectionalTarget>)mobActionSlice.action;

            yield return new JumpIn(mob.DoActionWithDefaultCosts(act, new FourDirectionalTarget(Consts.Direction.Up)));
        }
    }
}