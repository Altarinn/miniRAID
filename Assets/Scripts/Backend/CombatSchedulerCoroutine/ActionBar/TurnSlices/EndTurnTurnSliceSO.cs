using System.Collections;

namespace miniRAID.TurnSchedule
{
    public class EndTurnTurnSliceSO : TurnSliceSO
    {
        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            yield break;
        }
    }
}