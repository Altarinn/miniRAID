using System.Collections.Generic;
using miniRAID.Backend;
using Sirenix.Utilities;

namespace miniRAID
{
    public class BatchedRenderer : IStateRenderer
    {
        // TODO: Move me to another place
        public HashSet<IStateRenderer> renderers = new HashSet<IStateRenderer>();

        public virtual void Refresh()
        {
            renderers.ForEach(x => x.Refresh());
        }

        public virtual void Destroy()
        {
            renderers.ForEach(x => x.Destroy());
            renderers.Clear();
        }
    }
}