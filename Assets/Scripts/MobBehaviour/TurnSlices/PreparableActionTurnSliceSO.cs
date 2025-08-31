using System;
using System.Collections;
using System.Collections.Generic;
using miniRAID.Spells;
using miniRAID.TurnSchedule;
using Sirenix.OdinInspector;

namespace miniRAID.MobBehaviour.TurnSlices
{
    public abstract class PreparableActionTurnSliceSO : MobActionTurnSliceSO
    {
        [Title("Turn Slice Buffs")]
        [Tooltip("Buffs that will be applied when this turn slice is constructed")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public List<TurnSliceBuffSO> turnSliceBuffs = new List<TurnSliceBuffSO>();
        public override MobActionTurnSlice Wrap(MobData mob, RuntimeAction action, TurnSliceMetadata metadata)
        {
            return new PreparableActionTurnSlice(
                mob, action, this, metadata);
        }

        /// <summary>
        /// Will be called upon the constructor of PreparableActionTurnSlice.
        /// </summary>
        /// <param name="slice"></param>
        public virtual void OnConstruction(PreparableActionTurnSlice slice) 
        { 
            // Apply turn slice buffs
            ApplyTurnSliceBuffs(slice);
        }

        /// <summary>
        /// Applies all configured turn slice buffs to the mob and associates them with the turn slice.
        /// </summary>
        /// <param name="slice"></param>
        protected virtual void ApplyTurnSliceBuffs(PreparableActionTurnSlice slice)
        {
            if (turnSliceBuffs == null) return;

            foreach (var buffSO in turnSliceBuffs)
            {
                if (buffSO == null) continue;

                var buffInstance = slice.mob.AddListener(buffSO);
                if (buffInstance is TurnSliceBuff turnSliceBuff)
                {
                    turnSliceBuff.AssociateWithTurnSlice(slice);
                }
            }
        }

        /// <summary>
        /// Will be called after any mob used any action, in order to capture any potential changes.
        /// </summary>
        /// <param name="slice"></param>
        /// <param name="actionSource">The mob that just casted the action.</param>
        /// <param name="action">The action just being performed.</param>
        public virtual IEnumerator OnGlobalPostAction(
            PreparableActionTurnSlice slice, MobData actionSource, RuntimeAction action, SpellTarget target)
        {
            yield break;
        }
    }

    public class PreparableActionTurnSlice : MobActionTurnSlice
    {
        protected PreparableActionTurnSliceSO paSliceData => (PreparableActionTurnSliceSO)data;
        
        public PreparableActionTurnSlice(
            MobData mob, RuntimeAction action, AbstractTurnSliceSO data, TurnSliceMetadata metadata)
            : base(mob, action, data, metadata)
        {
            paSliceData.OnConstruction(this);
            Globals.backend.onGlobalActionPostcast.AddListener(OnGlobalPostAction);
        }

        protected virtual IEnumerator OnGlobalPostAction(
            MobData source, RuntimeAction action, SpellTarget target)
        {
            yield return new JumpIn(paSliceData.OnGlobalPostAction(this, source, action, target));
        }

        public override void OnRemove(CombatSchedulerCoroutine coroutine)
        {
            Globals.backend.onGlobalActionPostcast.RemoveListener(OnGlobalPostAction);
            base.OnRemove(coroutine);
        }
    }
}