using System.Collections;

namespace miniRAID.TurnSchedule
{
    public class EndTurnTurnSlice : TurnSlice
    {
        public override IEnumerator Turn()
        {
            yield break;
        }
    }
}