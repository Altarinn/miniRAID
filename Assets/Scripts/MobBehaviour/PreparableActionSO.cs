using System;
using miniRAID.TurnSchedule;
using Sirenix.OdinInspector;
using UnityEngine;

namespace miniRAID.MobBehaviour
{
    public abstract class PreparableActionSO : CustomIconScriptableObject
    {
        [InfoBox("No strong typing about SpellTarget here! Please fix PreparableActionSO.cs.", InfoMessageType.Warning)]
        // If corresponding action has already been added to the Mob,
        // level will be ignored. Otherwise, a new action with corre-
        // sponding level will be added to the Mob.
        [InfoBox("Level will be ignored if the action already exists on the mob.")]
        public ActionSOEntry action;
        
        public abstract void ModifySchedule(MobData mob, CombatSchedulerCoroutine coroutine);

        public abstract float GetWeight(MobData mob);

        public virtual bool ConditionCheck(MobData mob) => true;
    }
}