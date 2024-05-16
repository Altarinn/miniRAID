using System.Collections.Generic;
using System.Linq;

namespace miniRAID.TurnSchedule
{
    public class DefaultTurnGenerator : TurnSchedulerGeneratorBase
    {
        public List<TurnSliceSO> turnSlices;
        
        public override List<TurnSlice> GetNewTurn(Timestamp now)
        {
            return turnSlices.Select(x => x.Wrap(new TurnSliceMetadata(null))).ToList();
        }
    }
}