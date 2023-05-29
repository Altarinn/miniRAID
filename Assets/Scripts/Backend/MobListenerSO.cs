using UnityEngine;
using System.Collections;

namespace miniRAID
{
    public abstract class MobListenerSO : CustomIconScriptableObject
    {
        public enum ListenerType
        {
            Buff,
            Weapon,
            Armor,
            Accessory,

            /** Attachable things on top of weapon / armor etc. (e.g. Gems, ...) */
            Attachment,

            /** Mob Agent (The action controller of the actual mob, mainly enemies) */
            Agent,

            /** Job characteristics modifier, e.g. ForestElfMyth, FloraFairy, etc. */
            Passive,

            /** Action instances at runtime; Spells etc. */
            RuntimeAction,
        }

        // Basic properties

        public new string name;
        public ListenerType type;

        public int priority;

        public virtual MobListenerSO Clone()
        {
            return Instantiate(this);
        }

        public virtual MobListener Wrap(MobData parent)
        {
            return new MobListener(parent, this);
        }

        public virtual bool TryAdd(MobData target)
        {
            return true;
        }
    }

    [System.Serializable]
    public class MobListener : Backend.BackendState
    {
        [System.NonSerialized]
        public MobData parentMob;
        public MobListenerSO data;

        public virtual MobListenerSO.ListenerType type => data.type;
        public virtual string name => data.name;

        public MobListener(MobData parent, MobListenerSO data) { this.parentMob = parent; this.data = data; }

        public virtual bool TryAdd(MobData mob)
        {
            if(data == null) { return true; }
            return data.TryAdd(mob);
        }

        ////////////////////////////////////////////////////
        // MOBLISTENER EVENTS
        ////////////////////////////////////////////////////

        // Emitted when it "entered" a scene - being attached to a mob, mob attaching it entered a new scene, etc.
        public virtual void OnEnterScene(MobData mob) { }

        // Emitted before it is going to exit from a scene.
        public virtual void OnExitScene(MobData mob) { }

        // Emitted when attached - will not be triggered when a mob entered a new scene without removing the listener.
        // May play some animation but cannot block the main coroutine.
        public virtual void OnAttach(MobData mob)
        {
            this.parentMob = mob;
        }

        // Emitted before it is going to be removed from a mob.
        public virtual void OnRemove(MobData mob)
        {
            this.parentMob = null;
        }

        // Remove myself from mob
        public virtual void Destroy() 
        {
            if(this.parentMob != null)
            {
                this.parentMob.RemoveListener(this);
            }
        }

        public virtual string GetInformationString()
        {
            return data.name;
        }
    }
}
