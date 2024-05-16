using System.Collections.Generic;
using System.Linq;

namespace miniRAID.TurnSchedule
{
    public class SimpleMobBasedTurnGenerator : TurnSchedulerGeneratorBase
    {
        public List<TurnSliceSO> initialTurnSlices;
        
        public override List<TurnSlice> GetNewTurn(Timestamp now)
        {
            var turnSlices = initialTurnSlices.Select(x => x.Wrap(new TurnSliceMetadata(null))).ToList();

            var sortedMobs = Globals.backend.GetAllMobs()
                .Select(x => (x, x.GetTurnSliceModificationPriority(now, turnSlices)))
                .OrderByDescending(x => x.Item2)
                .Select(x => x.Item1)
                .ToList();
            
            foreach (MobData mob in sortedMobs)
            {
                mob.ModifyTurnSlicesInPlace(now, turnSlices);
            }

            return turnSlices;
        }
    }
}