using System;

namespace miniRAID.ActionHelpers
{
    public class NullListenerSO : MobListenerSO
    {
        public NullListenerSO() : base()
        {
            type = ListenerType.Internal;
        }

        public override MobListener Wrap(MobData parent)
        {
            throw new InvalidOperationException(
                "NullListenerSO cannot be wrapped. Directly call your listener's constructor instead.");
        }
    }
}