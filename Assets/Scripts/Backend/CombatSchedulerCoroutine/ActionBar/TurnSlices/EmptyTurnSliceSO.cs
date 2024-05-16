using System.Collections;

namespace miniRAID.TurnSchedule
{
    public class EmptyTurnSliceSO : TurnSliceSO
    {
        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            yield break;
        }
    }
}