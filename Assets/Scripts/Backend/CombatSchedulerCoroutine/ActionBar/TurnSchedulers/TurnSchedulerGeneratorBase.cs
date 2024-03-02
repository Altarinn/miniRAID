using System.Collections.Generic;

namespace miniRAID.TurnSchedule
{
    public abstract class TurnSchedulerGeneratorBase : CustomIconScriptableObject
    {
        public abstract List<TurnSlice> GetNewTurn();
    }
}