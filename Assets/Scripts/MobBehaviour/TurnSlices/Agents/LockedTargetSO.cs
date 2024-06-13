namespace miniRAID.Agents
{
    public class LockedTargetSO : MobListenerSO
    {
        public override MobListener Wrap(MobData parent)
        {
            return new LockedTarget(parent, this);
        }
    }

    public class LockedTarget : TargetIndicator
    {
        public LockedTarget(MobData parent, MobListenerSO data) : base(parent, data)
        {
        }
        
        public MobData target;
        public override MobData CurrentTarget => target;
    }
}