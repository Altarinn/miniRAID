using System;
using System.Collections;
using miniRAID.Agents;
using UnityEngine;

namespace miniRAID.TurnSchedule
{
    public abstract class MobTurnSliceBaseSO : AbstractTurnSliceSO
    {
        public virtual MobTurnSlice Wrap(MobData mob, TurnSliceMetadata metadata)
        {
            return new MobTurnSlice(mob, this, metadata);
        }
    }

    public class MobTurnSlice : TurnSlice
    {
        public MobData mob;

        public override string Label => $"{mob.nickname}'s Turn";

        public MobTurnSlice(MobData mob, AbstractTurnSliceSO data, TurnSliceMetadata metadata) : base(data, metadata)
        {
            this.mob = mob;
        }
    }
}