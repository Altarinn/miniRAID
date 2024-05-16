using System.Collections;

namespace miniRAID.TurnSchedule
{
    public class StartTurnTurnSliceSO : TurnSliceSO
    {
        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            // TODO: Move logic to coroutine?
            coroutine.now.currentTurnID++;
            Globals.combatTracker.Turns = coroutine.now.currentTurnID;
            yield break;
        }
    }
}