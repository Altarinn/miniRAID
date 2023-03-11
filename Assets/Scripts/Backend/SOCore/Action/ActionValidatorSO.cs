using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Sirenix.OdinInspector;

namespace miniRAID
{
    [CreateAssetMenu(menuName = "Action/ActionValidator")]
    public class ActionValidatorSO : CustomIconScriptableObject
    {
        ActionDataSO action;

        [TypeFilter("GetRequesterTypes")]
        public UI.TargetRequester.TargetRequesterBase Requester;

        public virtual bool Equipable(MobData mobdata) { return true; }
        public virtual bool Check(Mob mob) { return true; }

        public virtual bool CheckWithTargets(Mob mob, Spells.SpellTarget target)
        {
            // TODO
            return true;
        }

        public IEnumerable<System.Type> GetRequesterTypes()
        {
            var q = typeof(UI.TargetRequester.TargetRequesterBase).Assembly.GetTypes()
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsGenericTypeDefinition)
            .Where(x => typeof(UI.TargetRequester.TargetRequesterBase).IsAssignableFrom(x));

            return q;
        }
    }
}
