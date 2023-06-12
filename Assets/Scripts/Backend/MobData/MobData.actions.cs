using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID
{
    public partial class MobData
    {
        #region Manipulating actions
        
        public RuntimeAction AddAction(ActionDataSO actSO)
        {
            var ract = new RuntimeAction(this, actSO);
            ract.SetData(actSO);
            actions.Add(ract);
            AddListener(ract);

            return ract;
        }
        
        /// <summary>
        /// Get the first available action with action name.
        /// </summary>
        /// <param name="ActionName"></param>
        /// <returns></returns>
        public RuntimeAction GetAction(string ActionName)
        {
            foreach (var act in availableActions)
            {
                if(act.data.ActionName == ActionName)
                {
                    return act;
                }
            }

            return null;
        }

        public RuntimeAction GetAction(ActionDataSO action)
        {
            foreach (var act in availableActions)
            {
                if (act.data == action)
                {
                    return act;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the first action of class specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public RuntimeAction GetAction<T>() where T : RuntimeAction
        {
            foreach (var act in availableActions)
            {
                if (act is T)
                {
                    return act;
                }
            }

            return null;
        }

        #endregion
        
        #region Performing actions
        
        public IEnumerator MoveToCoroutine(Vector3Int targetPos, GridPath path)
        {
            // Check if targetPos is valid; If not, terminate the movement
            if (!Globals.backend.CanGridPlaceMob(targetPos, gridBody))
            {
                yield return -1;
            }
            
            // TODO: implement path for field effects (move w.r.t. the path & tell backend that we reached a intermediate point)
            if (Globals.cc.animation && mobRenderer != null)
                yield return new JumpIn(mobRenderer.MoveTowards(targetPos));

            // Tell backend that we finished the movement

            // TODO: change to use path
            UseActionPoint(
                (Mathf.Abs(targetPos.x - Position.x)
                 + Mathf.Abs(targetPos.y - Position.y)
                 + Mathf.Abs(targetPos.z - Position.z)) / 2.0f);
            Position = targetPos;
        }
        
        public IEnumerator ActionPrecheck(RuntimeAction raction, Spells.SpellTarget target)
        {
            yield return new JumpIn(OnActionChosen?.Invoke(this, raction, target));
        }

        public IEnumerator ActionBegin(RuntimeAction raction, Spells.SpellTarget target)
        {
            yield return new JumpIn(OnActionPrecast?.Invoke(this, raction, target));
        }

        public IEnumerator ActionDone(RuntimeAction raction, Spells.SpellTarget target)
        {
            yield return new JumpIn(OnActionPostcast?.Invoke(this, raction, target));

            RecalculateStats();
        }
        
        //public Cost GetDisplayCost(Cost cost, RuntimeAction ract)
        //{
        //    // TODO: Dummy action
        //    OnCostQueryDisplay?.Invoke(cost, ract, this);
        //    return cost;
        //}
        
        public Cost GetModifiedCost(Cost cost, RuntimeAction ract)
        {
            // TODO: Dummy action
            OnCostQuery?.Invoke(cost, ract, this);
            return cost;
        }
        
        public bool CheckCost(Cost cost, RuntimeAction ract)
        {
            // TODO: Dummy action
            var mCost = GetModifiedCost(cost, ract);

            switch(mCost.type)
            {
                case Cost.Type.AP:
                    return cost.value <= actionPoints;
                    break;
                case Cost.Type.Mana:
                    Debug.LogWarning("Please implement CheckCost for Mana.");
                    break;
                case Cost.Type.CD:
                    return ract.cooldownRemain <= 0;
                    break;
            }

            return true;
        }
        
        public IEnumerator ApplyCost(Cost cost, RuntimeAction ract)
        {
            yield return new JumpIn(OnCostApply?.Invoke(cost, ract, this));

            switch (cost.type)
            {
                case Cost.Type.AP:
                    UseActionPoint(cost.value);
                    break;
                case Cost.Type.Mana:
                    Debug.LogWarning("Please implement ApplyCost for Mana.");
                    break;
                case Cost.Type.CD:
                    ract.SetCoolDown(cost.value);
                    break;
            }

            yield break;
        }

        public IEnumerator DoAction(
            ActionDataSO actionSO,
            Spells.SpellTarget target,
            List<Cost> costs = null)
        {
            var ra = GetAction(actionSO) ?? AddAction(actionSO);
            yield return new JumpIn(DoAction(ra, target, costs));
        }
        
        public IEnumerator DoAction(
            RuntimeAction raction,
            Spells.SpellTarget target,
            List<Cost> costs = null)
        {
            // Needs this?
            //RecalculateStats();

            yield return new JumpIn(ActionPrecheck(raction, target));
            
            // Do Cost Check Stuffs
            if(costs != null)
            {
                // Check all costs
                if(costs.Select(cost => CheckCost(cost, raction)).Any(x => !x))
                {
                    // If some cost cannot be satisfied
                    // Stop using the action
                    yield break;
                }

                foreach (Cost cost in costs)
                {
                    yield return new JumpIn(ApplyCost(cost, raction));
                }
            }
            
            Globals.logger?.Log($"[DoAction] {nickname} attempts casting {raction.data.name} towards {target.targetPos[0]}");
            Globals.combatTracker.Record(new Consts.TrackerActionEvent()
            {
                action = raction,
                target = target
            });

            yield return new JumpIn(ActionBegin(raction, target));
            yield return new JumpIn(raction.Activate(this, target));
            yield return new JumpIn(ActionDone(raction, target));
        }
        
        public IEnumerator DoActionWithDefaultCosts(
            RuntimeAction raction,
            Spells.SpellTarget target)
        {
            List<Cost> cost = raction.data.costs.Select(pair =>
                new Cost(dNumber.CreateComposite(pair.Value.Eval((this, target))), pair.Key)).ToList();
            yield return new JumpIn(DoAction(raction, target, cost));
        }
        
        internal bool CheckCalculatedActionCostBounds(RuntimeAction action)
        {
            return action.costBounds.Select(cost => CheckCost(cost.Item1, action)).All(x => x);
        }
        
        // Play an animation on MobRenderer instance and wait for complete.
        // If mobRenderer == null, this coroutine will end immediately.
        public IEnumerator WaitForAnimation(string animationState)
        {
            if (mobRenderer != null)
                yield return new JumpIn(mobRenderer.WaitForAnimation(animationState));
        }
        
        #endregion
    }
}