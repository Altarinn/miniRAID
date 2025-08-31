using miniRAID.Backend;
using miniRAID.Buff;
using miniRAID.TurnSchedule;
using UnityEngine;

namespace miniRAID.MobBehaviour.TurnSlices
{
    /// <summary>
    /// Base class for buffs that are associated with specific turn slices.
    /// These buffs can terminate their associated turn slice when certain conditions are met.
    /// </summary>
    public abstract class TurnSliceBuffSO : BuffSO
    {
        public override MobListener Wrap(MobData parent)
        {
            return WrapTurnSliceBuff(parent);
        }

        protected abstract TurnSliceBuff WrapTurnSliceBuff(MobData parent);
    }

    /// <summary>
    /// Runtime implementation of turn slice buffs.
    /// Provides functionality to terminate the associated turn slice.
    /// </summary>
    public abstract class TurnSliceBuff : Buff.Buff
    {
        protected PreparableActionTurnSlice associatedTurnSlice;

        public TurnSliceBuff(MobData source, TurnSliceBuffSO data) : base(source, data)
        {
        }

        /// <summary>
        /// Associates this buff with a specific turn slice.
        /// Called by PreparableActionTurnSlice during construction.
        /// </summary>
        public virtual void AssociateWithTurnSlice(PreparableActionTurnSlice turnSlice)
        {
            associatedTurnSlice = turnSlice;
        }

        /// <summary>
        /// Terminates the associated turn slice by muting it.
        /// </summary>
        protected virtual void TerminateAssociatedTurnSlice()
        {
            if (associatedTurnSlice != null)
            {
                associatedTurnSlice.Mute();
            }
        }
    }
}