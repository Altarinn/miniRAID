using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using miniRAID.TurnSchedule;
using UnityEngine;

namespace miniRAID
{
    public partial class CombatSchedulerCoroutine
    {
        // TODO: Serialize the object data but not the full object. Hard to manage serialization on-the-fly for MonoBehaviours.
        #region Serialization

        public struct SerializableInfo
        {
            public List<TurnSlice> serializedTurnSchedule;
            public TurnSlice currentTurnSlice;
            public TurnSchedulerGeneratorBase turnScheduler;
            public Timestamp now, appendedTurns;
        }

        public SerializableInfo PrepareSerializationInfo()
        {
            return new SerializableInfo()
            {
                serializedTurnSchedule = turnSchedule.ToList(),
                currentTurnSlice = currentTurnSlice,
                now = now,
                appendedTurns = appendedTurns,
                turnScheduler = turnScheduler
            };
        }

        public void RestoreFromSerialization(SerializableInfo info)
        {
            turnSchedule = new TurnScheduleSequence(info.serializedTurnSchedule)
            {
                parentScheduler = this
            };
            
            foreach (var turnSlice in turnSchedule)
            {
                turnSlice.RegisterTo(this);
            }
            
            turnScheduler = info.turnScheduler;
            currentTurnSlice = info.currentTurnSlice;
            now = info.now;
            appendedTurns = info.appendedTurns;

            // turnEnd = true; // ?
            playerPhaseEnd = false;
        }
        
        #endregion
        
    }
}