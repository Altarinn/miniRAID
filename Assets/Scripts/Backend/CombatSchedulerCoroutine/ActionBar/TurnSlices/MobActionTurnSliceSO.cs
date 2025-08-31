using System.Collections;

namespace miniRAID.TurnSchedule
{
    public abstract class MobActionTurnSliceSO : AbstractTurnSliceSO
    {
        public virtual MobActionTurnSlice Wrap(MobData mob, RuntimeAction action, TurnSliceMetadata metadata)
        {
            if (!ScheduleFilter(metadata)) return null;
            return new MobActionTurnSlice(mob, action, this, metadata);
        }
    }

    public class MobActionTurnSlice : TurnSlice
    {
        public MobData mob;
        public RuntimeAction action;

        public override string Label => $"{mob.nickname}: {action.ActionName}";

        public MobActionTurnSlice(
            MobData mob, RuntimeAction action, AbstractTurnSliceSO data, TurnSliceMetadata metadata)
            : base(data, metadata)
        {
            this.mob = mob;
            this.action = action;
        }
    }
}