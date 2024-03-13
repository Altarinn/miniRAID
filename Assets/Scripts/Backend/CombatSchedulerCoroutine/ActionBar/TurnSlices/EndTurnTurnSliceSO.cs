using System.Collections;

namespace miniRAID.TurnSchedule
{
    public class EndTurnTurnSliceSO : TurnSliceSO
    {
        public override IEnumerator Turn(CombatSchedulerCoroutine coroutine)
        {
            yield break;
        }
    }
}