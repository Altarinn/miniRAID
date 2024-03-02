using System.Collections;

namespace miniRAID.TurnSchedule
{
    public class StartTurnTurnSlice : TurnSlice
    {
        public override IEnumerator Turn()
        {
            // TODO: Move logic to coroutine?
            coroutine.turn++;
            Globals.combatTracker.Turns = coroutine.turn;
            yield break;
        }
    }
}