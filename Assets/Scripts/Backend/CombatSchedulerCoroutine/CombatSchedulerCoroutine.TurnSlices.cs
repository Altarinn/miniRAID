using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Math;
using miniRAID.TurnSchedule;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace miniRAID
{
    public class LinkedListQueue<T> : LinkedList<T>
    {
        public LinkedListQueue() : base() { }

        public LinkedListQueue(IEnumerable<T> xs) : base(xs)
        {}
        
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

        public IEnumerable<LinkedListNode<T>> Where(System.Func<LinkedListNode<T>, bool> condition)
        {
            for (var node = First;
                 node != null;
                 node = node.Next)
            {
                if (condition.Invoke(node))
                {
                    yield return node;
                }
            }
        }
    }

    public class TurnScheduleSequence : LinkedListQueue<TurnSlice>
    {
        public CombatSchedulerCoroutine parentScheduler;

        public TurnScheduleSequence() : base() { }
        
        public TurnScheduleSequence(IEnumerable<TurnSlice> slices) : base(slices)
        {}
        
        public void InsertTurnSliceAt(int index, TurnSlice slice)
        {
            // Insert at the desired position
            for (var node = First;
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

            // If we didn't find the position (index >= len), append at last instead
            AddLast(slice);
            return;
        }

        public void InsertTurnSliceBefore(LinkedListNode<TurnSlice> node, TurnSlice slice)
        {
            slice.RegisterTo(parentScheduler);
            
            // TODO: timestamp might be problematic. Ignored for now.
            slice.metadata.timestamp.currentTurnID = node.Value.metadata.timestamp.currentTurnID;
            AddBefore(node, slice);
        }
        
        public void InsertTurnSliceRightAfter(LinkedListNode<TurnSlice> node, TurnSlice slice)
        {
            slice.RegisterTo(parentScheduler);
            
            // TODO: timestamp might be problematic. Ignored for now.
            slice.metadata.timestamp.currentTurnID = node.Value.metadata.timestamp.currentTurnID;
            AddAfter(node, slice);
        }
        
        // Tries to maintain an order based on priority within same category block.
        // Rule: 1) Descending 2) ---> Added early ---> Added later ---> when priority equals.
        public void InsertTurnSliceAfter(LinkedListNode<TurnSlice> pivotNode, TurnSlice slice)
        {
            slice.RegisterTo(parentScheduler);

            var node = pivotNode;
            int priority = slice.metadata.Priority.Eval(null);
            while (node != null)
            {
                var nextNode = node.Next;

                if (nextNode == null)
                {
                    break;
                }

                if (nextNode.Value.metadata.Priority.Eval(null) < priority ||
                    nextNode.Value.metadata.category != slice.metadata.category)
                {
                    break;
                }

                node = nextNode;
            }

            // TODO: timestamp might be problematic. Ignored for now.
            slice.metadata.timestamp.currentTurnID = node.Value.metadata.timestamp.currentTurnID;
            AddAfter(node, slice);
        }

        public void RemoveAllTurnSlicesFrom(object source)
        {
            var node = First;
            while (node != null)
            {
                var nextNode = node.Next;
                if (node.Value.metadata.source == source)
                {
                    Remove(node);
                }

                node = nextNode;
            }
        }

        /// <summary>
        /// Sorts all turnslices in current schedule by their priority & categories.
        /// </summary>
        public void SortTurnSlicesByCategory()
        {
            var node = First;
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

    // TODO: Implement own LinkedList to support fancier operations, fxxk
    public partial class CombatSchedulerCoroutine
    {
        public TurnScheduleSequence turnSchedule;

        [SerializeField] private TurnSchedulerGeneratorBase turnScheduler;

        public void InitializeTurnSchedule()
        {
            turnScheduler = FindObjectOfType<TurnSchedulerComponent>()?.scheduler ?? turnScheduler;
            turnSchedule = new TurnScheduleSequence();
            turnSchedule.parentScheduler = this;

            appendedTurns.currentTurnID = 1;
            appendedTurns.currentTurnSliceID = 0;
            
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
            var newTurn = turnScheduler.GetNewTurn(ref appendedTurns);
            
            foreach (var turn in newTurn)
            {
                turn.RegisterTo(this);
                turnSchedule.Enqueue(turn);
            }
        }
    }
}