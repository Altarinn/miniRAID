using System.Collections;
using miniRAID.ActionHelpers;
using miniRAID.Agents;
using miniRAID.Backend;
using miniRAID.Spells;
using miniRAID.TurnSchedule;
using Sirenix.Serialization;

namespace miniRAID.MobBehaviour.TurnSlices
{
    public abstract class SimpleUseTargetedActionTurnSliceSO : PreparableActionTurnSliceSO
    {
        public bool UpdateTargetAfterInitialized = false;
    }

    public abstract class SimpleUseTargetedActionTurnSlice<T> : PreparableActionTurnSlice, IRenderableState
        where T : SpellTarget
    {
        [OdinSerialize] protected T target;

        public SimpleUseTargetedActionTurnSlice(
            MobData mob, RuntimeAction action, AbstractTurnSliceSO data, TurnSliceMetadata metadata)
            : base(mob, action, data, metadata)
        {
            target = GetTarget(mob);
        }

        protected abstract T GetTarget(MobData self);

        protected override IEnumerator OnGlobalPostAction(MobData source, RuntimeAction action, SpellTarget target)
        {
            if (!((SimpleUseTargetedActionTurnSliceSO)data).UpdateTargetAfterInitialized)
            {
                yield break;
            }

            if (action == this.action)
            {
                yield break;
            }

            if (mob.isDead)
            {
                Mute();
            }

            this.target = GetTarget(mob);
        }

        public override IEnumerator Turn()
        {
            if (mob.isDead)
            {
                yield break;
            }

            renderer?.Destroy();
            renderer = null;

            if (target == null) yield break;

            RuntimeAction<T> act = (RuntimeAction<T>)action;
            yield return new JumpIn(mob.DoActionWithDefaultCosts(act, target));
        }

        public abstract void ConstructRenderer();
        public abstract void UpdateRenderer();
    }
}