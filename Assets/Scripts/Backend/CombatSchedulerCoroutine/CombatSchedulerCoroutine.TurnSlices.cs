using System.Collections.Generic;
using System.Linq;
using miniRAID.TurnSchedule;
using UnityEngine;

namespace miniRAID
{
    public partial class CombatSchedulerCoroutine
    {
        private Queue<TurnSlice> turnSchedule;

        [SerializeField] private TurnSchedulerGeneratorBase turnScheduler;

        public void InitializeTurnSchedule()
        {
            turnSchedule = new Queue<TurnSlice>();
            KeepTurnScheduleLength();
        }
        
        public void KeepTurnScheduleLength(int length = 10)
        {
            while (turnSchedule.Count <= length)
            {
                AppendNewTurn();
            }
        }
        
        public void AppendNewTurn()
        {
            var newTurn = turnScheduler.GetNewTurn();
            newTurn.ForEach(x =>
            {
                x.RegisterTo(this);
                turnSchedule.Enqueue(x);
            });
        }

        public List<TurnSlice> _TurnScheduleView => turnSchedule?.ToList();
    }
}