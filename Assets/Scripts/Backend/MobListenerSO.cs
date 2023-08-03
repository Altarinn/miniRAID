using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;

namespace miniRAID
{
    [System.Serializable]
    public struct MobListenerSOEntry
    {
        public MobListenerSO data;
        public int level;
    }
    
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
            
            /** Resources */
            Resource,
            
            Internal
        }

        // Basic properties

        public LocalizedString nameKey;
        public new string name => Globals.localizer.L(nameKey) ?? "BAD_STRING";
        
        public ListenerType type;

        public int priority;

        public virtual MobListenerSO Clone()
        {
            return Instantiate(this);
        }

        // Why do we need "parent" argument right now?
        public virtual MobListener Wrap(MobData parent)
        {
            return new MobListener(parent, this);
        }

        public virtual MobListener LeveledWrap(MobData parent, int level)
        {
            var ml = Wrap(parent);
            ml.level = level;
            
            return ml;
        }

        public virtual bool TryAdd(MobData target)
        {
            return true;
        }
    }

    // [System.Serializable]
    public class MobListener : Backend.BackendState
    {
        [System.NonSerialized]
        public MobData parentMob;
        
        public int level = 0;
        
        // TODO: FIXME: This is getting hid in derived classes (e.g., Weapon, ActionDataSO, etc.), resulting in unexpected behaviors when calling data.xxx.
        public MobListenerSO data;

        public virtual MobListenerSO.ListenerType type => data.type;
        public virtual string name => data?.name;
        public virtual string ShortTooltip => "NO_SHORT_TOOLTIP";

        public HashSet<IMobListenerIndicator> indicators;

        public MobListener(MobData parent, MobListenerSO data) { this.parentMob = parent; this.data = data; }

        public virtual bool TryAdd(MobData mob)
        {
            if(data == null) { return true; }
            return data.TryAdd(mob);
        }

        public T AddIndicator<T>(T indicator) where T : IMobListenerIndicator
        {
            if (indicators == null)
            {
                indicators = new();
            }
            
            if(indicators.Add(indicator))
            {
                indicator.Instantiate();
                return indicator;
            }

            return default;
        }

        public void RemoveIndicator(IMobListenerIndicator indicator)
        {
            if (indicators.Remove(indicator))
            {
                indicator.Destroy();
            }
        }
        
        public void RemoveAllIndicators()
        {
            if (indicators == null)
            {
                return;
            }
            
            foreach(var indicator in indicators)
            {
                indicator.Destroy();
            }
            
            indicators.Clear();
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
            // Destroy all indicators
            RemoveAllIndicators();
            
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
