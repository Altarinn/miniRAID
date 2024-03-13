using System.Collections.Generic;
using System.Linq;
using miniRAID.TurnSchedule;
using UnityEngine;

namespace miniRAID
{
    public class LinkedListQueue<T> : LinkedList<T>
    {
        public void Enqueue(T x)
        {
            this.AddLast(x);
        }

        public T Dequeue()
        {
            T x = First.Value;
            this.RemoveFirst();
            return x;
        }
    }
    
    public partial class CombatSchedulerCoroutine
    {
        public LinkedListQueue<TurnSlice> turnSchedule;

        [SerializeField] private TurnSchedulerGeneratorBase turnScheduler;

        public void InitializeTurnSchedule()
        {
            turnSchedule = new LinkedListQueue<TurnSlice>();
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

        public void InsertTurnSliceAt(int index, TurnSlice slice)
        {
            // Insert at the desired position
            for (var node = turnSchedule.First;
                 node != null;
                 node = node.Next)
            {
                if (index <= 0)
                {
                    turnSchedule.AddBefore(node, slice);
                    return;
                }
                index--;
            }

            // If we didn't find the position (index >= turnSchedule.len), append at last instead
            turnSchedule.AddLast(slice);
            return;
        }
    }
}