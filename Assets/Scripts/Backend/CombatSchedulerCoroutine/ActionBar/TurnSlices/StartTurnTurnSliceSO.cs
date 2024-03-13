using System.Collections;

namespace miniRAID.TurnSchedule
{
    public class StartTurnTurnSliceSO : TurnSliceSO
    {
        public override IEnumerator Turn(CombatSchedulerCoroutine coroutine)
        {
            // TODO: Move logic to coroutine?
            coroutine.turn++;
            Globals.combatTracker.Turns = coroutine.turn;
            yield break;
        }
    }
}