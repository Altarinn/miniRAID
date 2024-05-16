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

    public class TurnScheduleSequence : LinkedListQueue<TurnSlice>
    { }

    // TODO: Implement own LinkedList to support fancier operations, fxxk
    public partial class CombatSchedulerCoroutine
    {
        public TurnScheduleSequence turnSchedule;

        [SerializeField] private TurnSchedulerGeneratorBase turnScheduler;

        public void InitializeTurnSchedule()
        {
            turnScheduler = FindObjectOfType<TurnSchedulerComponent>()?.scheduler ?? turnScheduler;
            turnSchedule = new TurnScheduleSequence();
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
            // TODO: Timestamp is not correct!!!
            // TODO: Provide timestamp that is later than "now" and actually corresponds to the new turns
            Debug.LogError("Timestamp is not correct.");
            var newTurn = turnScheduler.GetNewTurn(now);
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
                    InsertTurnSliceBefore(node, slice);
                    return;
                }
                index--;
            }

            // If we didn't find the position (index >= turnSchedule.len), append at last instead
            turnSchedule.AddLast(slice);
            return;
        }

        public void InsertTurnSliceBefore(LinkedListNode<TurnSlice> node, TurnSlice slice)
        {
            slice.RegisterTo(this);
            turnSchedule.AddBefore(node, slice);
        }

        public void RemoveAllTurnSlicesFrom(object source)
        {
            var node = turnSchedule.First;
            while (node != null)
            {
                var nextNode = node.Next;
                if (node.Value.metadata.source == source)
                {
                    turnSchedule.Remove(node);
                }

                node = nextNode;
            }
        }

        /// <summary>
        /// Sorts all turnslices in current schedule by their priority & categories.
        /// </summary>
        public void SortTurnSlicesByCategory()
        {
            var node = turnSchedule.First;
            TurnSliceCategory prevCategory = node.Value.metadata.category;
            var prevNode = node;
            
            while (node != null)
            {
                if (prevCategory != node.Value.metadata.category)
                {
                    // TODO: Remove from schedule
                    // turnSchedule
                }
            }
        }
    }
}