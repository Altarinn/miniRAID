using System.Collections.Generic;
using System.Linq;

namespace miniRAID.TurnSchedule
{
    public class SimpleMobBasedTurnGenerator : TurnSchedulerGeneratorBase
    {
        public List<TurnSliceSO> initialTurnSlices;
        
        public override TurnScheduleSequence GetNewTurn(ref Timestamp now)
        {
            Timestamp copiedNow = now;
            
            TurnScheduleSequence turnSlices = new(
                initialTurnSlices.Select(x =>
                {
                    var xx = x.Wrap(new TurnSliceMetadata(null));
                    xx.metadata.timestamp = new Timestamp(copiedNow.currentTurnID);
                    return xx;
                }));

            var sortedMobs = Globals.backend.GetAllMobs()
                .Select(x => (x, x.GetTurnSliceModificationPriority(copiedNow, turnSlices)))
                .OrderByDescending(x => x.Item2)
                .Select(x => x.Item1)
                .ToList();
            
            foreach (MobData mob in sortedMobs)
            {
                mob.ModifyTurnSlicesInPlace(now, turnSlices);
            }

            now.currentTurnID += 1;

            return turnSlices;
        }
    }
}