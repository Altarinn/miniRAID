using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace miniRAID
{
    public enum GCDGroup
    {
        None,
        Common,
        Special
    }

    [Obsolete("Check ActionDataSO and RuntimeAction instead.")]
    public abstract class Action : CustomIconScriptableObject
    {
        public string ActionName;
        [TextArea]
        public string Description;
        public List<string> Tags;

        public int ManaCost;
        public int ActionPointCost;
        public int Cooldown;

        public int CooldownRemain;

        public bool isActivelyUsed = true;
        public GCDGroup GCDgroup = GCDGroup.Common;

        // TODO: custom inspector for this
        // Now just use simple GUI
        [HideInInspector]
        public UI.TargetRequester.TargetRequesterBase Requester;

        public delegate void OnActionPerformed(Action action, MobData source);
        public delegate void OnActionPerformFailed(Action action, MobData source);

        public delegate void OnActionCorotinePreExecute();
        public delegate void OnActionCorotineFinished();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mob"></param>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        /// <param name="cd"></param>
        /// <param name="host">If true, callback will be invoked after all actions have finished (not only this action, also actions that invoked by this; e.g. shield effect, counter-attacks, etc.)</param>
        /// <returns></returns>
        protected virtual bool Do(MobData mob, Spells.SpellTarget target, OnActionCorotineFinished callback = null, bool cd = true, bool host = false)
        {
            throw new System.NotImplementedException();
            //if (cd)
            //{
            //    // Start spell cooldown
            //    CooldownRemain = Cooldown;
            //}

            //bool shallDo = PreCorotine(mob, target);
            //if (!shallDo) { return false; }

            //mob.ActionBegin(this, target);

            //if (host)
            //{
            //    Globals.actionHost.Instance.HostActionCoroutine(Coroutine(mob, target), () => 
            //        { mob.ActionDone(this, target); callback?.Invoke(); }
            //    );
            //}
            //else
            //{
            //    Globals.actionHost.Instance.StartActionCoroutine(Coroutine(mob, target), () =>
            //        { mob.ActionDone(this, target); callback?.Invoke(); }
            //    );
            //}

            //return true;
        }

        protected abstract IEnumerator Coroutine(MobData mob, Spells.SpellTarget target);
        protected virtual bool PreCorotine(MobData mob, Spells.SpellTarget target) { return true; }

        /// <summary>
        /// Check if the action performable for a given mob.
        /// This should not contain any resources (cooldown, mana, action point, gcd, etc.) as the action may triggered freely from other spells.
        /// </summary>
        /// <param name="mob"></param>
        /// <returns></returns>
        public virtual bool Check(MobData mob)
        {
            return true;
        }

        /// <summary>
        /// Check if action performable with costs.
        /// </summary>
        /// <param name="mob"></param>
        /// <returns></returns>
        public virtual bool CheckWithCosts(MobData mob)
        {
            return false;
            // TODO: Mana check, AP check, etc.
            //return (
            //    Check(mob) &&
            //    mob.data.availableActions.Contains(this) &&
            //    !mob.data.IsInGCD(this.GCDgroup) &&
            //    CooldownRemain <= 0
            //);
        }

        /// <summary>
        /// Check is action performable with targets.
        /// Usually the final check.
        /// </summary>
        /// <param name="mob"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool CheckWithTargets(MobData mob, Spells.SpellTarget target)
        {
            return Check(mob);
        }

        public virtual bool DoCost(MobData mob, Spells.SpellTarget target)
        {
            // TODO: Mana
            if(
                mob.actionPoints >= ActionPointCost &&
                !mob.IsInGCD(this.GCDgroup)
            )
            {
                mob.UseActionPoint(ActionPointCost);
                mob.SetGCD(this.GCDgroup);

                return true;
            }

            return false;
        }

        public virtual void Activate(MobData mob, Spells.SpellTarget target, OnActionCorotineFinished callback = null, bool host = false, bool cd = true)
        {
            if(CheckWithTargets(mob, target))
            {
                Do(mob, target, callback, cd, host);
            }
            else
            {
                callback?.Invoke();
            }
        }

        public virtual void ActivateInUI(
            MobData mob, 
            OnActionCorotinePreExecute beforeAnimation, 
            OnActionCorotineFinished afterAnimation, 
            OnActionPerformed callback = null)
        {
            if(Check(mob))
            {
                Requester.Request(mob, null, (Spells.SpellTarget target) =>
                {
                    //mob.DoAction(this, target, beforeAnimation, afterAnimation, callback);
                }, () => { });
            }
        }

        public virtual string GetToolTip()
        {
            return Description;
        }

        public virtual void OnNextTurn(MobData mob)
        {
            CooldownRemain -= 1;
            if (CooldownRemain < 0) { CooldownRemain = 0; }
        }

        public override int GetHashCode()
        {
            return ActionName.GetHashCode();
        }
    }
}