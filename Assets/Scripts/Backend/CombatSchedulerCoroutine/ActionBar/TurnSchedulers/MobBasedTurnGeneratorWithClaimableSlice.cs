using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule
{
    public class MobBasedTurnGeneratorWithClaimableSlice : TurnSchedulerGeneratorBase
    {
        [SerializeField]
        private List<AbstractTurnSliceSO> initialTurnSlices;

        public override TurnScheduleSequence GetNewTurn(ref Timestamp now)
        {
            Timestamp copiedNow = now;

            TurnScheduleSequence turnSlices = new(
                initialTurnSlices.Select(x =>
                {
                    TurnSliceMetadata meta = new TurnSliceMetadata(null);
                    meta.timestamp = new Timestamp(copiedNow.currentTurnID);

                    // Wrap it dynamically
                    var xx = DynamicWrap(x, meta);
                    return xx;
                }).Where(x => x != null));

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

        public TurnSlice DynamicWrap(AbstractTurnSliceSO absTurnSliceSO, TurnSliceMetadata metadata)
        {
            if(absTurnSliceSO is TurnSliceSO turnSliceSO)
            {
                return turnSliceSO.Wrap(metadata);
            }
            else
            {
                return absTurnSliceSO.DummyWrap(metadata);
            }
        }
    }
}