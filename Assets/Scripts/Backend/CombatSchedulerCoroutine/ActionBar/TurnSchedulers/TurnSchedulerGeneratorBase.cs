using System.Collections.Generic;

namespace miniRAID.TurnSchedule
{
    public abstract class TurnSchedulerGeneratorBase : CustomIconScriptableObject
    {
        public abstract TurnScheduleSequence GetNewTurn(ref Timestamp now);
    }
}