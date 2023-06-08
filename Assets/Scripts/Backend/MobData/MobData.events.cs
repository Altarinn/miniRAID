using System.Collections;
using System.Collections.Generic;
using System.Linq;
using miniRAID;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace miniRAID
{
    public partial class MobData
    {
        ///////////////////////////// Delegates /////////////////////////////

        public delegate void MobArgumentDelegate(MobData mob);
        
        // TODO: ?
        public delegate void MobMenuGUIDelegate(MobData data, UI.UnitMenu state, UI.UIMenu_UIContainer menu);

        public delegate void CostQueryDelegate(Cost cost, RuntimeAction ract, MobData mob);
        // WTF??? How about mana-draining attacks?
        // Cost = Damage ?

        // TODO: Think about this signature
        public delegate void MobActionQueryDelegate(MobData mob, HashSet<RuntimeAction> actions);
        
        /////////////////////////////  Events  //////////////////////////////
        /// TODO: Invoke order ......

        //// Timing

        // Before status re-calculation; 
        //public event MobArgumentDelegate OnEarlyWakeup;

        // Emitted when the mob has its active state turned from "slept / 待机" to "awake / 可行动", after resetted all related parameters.
        public CoroutineEvent<MobData> OnWakeup;

        // Used for agents, after the regular wakeup process
        public CoroutineEvent<MobData> OnAgentWakeUp;

        // Emitted when the mob has entered a new turn ("PHASE" for this mob has started)
        public CoroutineEvent<MobData> OnNextTurn;
        
        //// Status calculation

        public event MobArgumentDelegate OnBaseStatCalculation;
        public event MobArgumentDelegate OnStatCalculation;
        public event MobArgumentDelegate OnActionStatCalculation;
        public event MobArgumentDelegate OnStatCalculationFinish;
        public event MobArgumentDelegate OnMobMoved;

        public event MobActionQueryDelegate OnQueryActions;
        
        //// User Interface
        // TODO: ?
        public event MobMenuGUIDelegate OnShowMobMenu;
        
        //// Combat-related

        // Emitted when the mob dealing a damage as the source.
        public CoroutineEvent<MobData, Consts.DamageHeal_FrontEndInput_ByRef> OnDealDmg;
        // public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnDealDamageFinal;
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnDamageDealt;

        public CoroutineEvent<MobData, Consts.DamageHeal_FrontEndInput_ByRef> OnDealHeal;
        // public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnDealHealFinal;
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnHealDealt;

        public CoroutineEvent<MobData, Consts.DamageHeal_FrontEndInput_ByRef> OnReceiveDamage;
        // public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnReceiveDamageFinal;
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnDamageReceived;

        public CoroutineEvent<MobData, Consts.DamageHeal_FrontEndInput_ByRef> OnReceiveHeal;
        // public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnReceiveHealFinal;
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnHealReceived;

        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnKill; // TODO: ByRef?
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnPreDeath; // TODO: ByRef?
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnRealDeath;

        // When we still don't know if the action can be performed or not, but targets already pick'd
        public CoroutineEvent<MobData, RuntimeAction, Spells.SpellTarget> OnActionChosen;
        // Action ready, confirmed okay to be performed, but not performed yet
        public CoroutineEvent<MobData, RuntimeAction, Spells.SpellTarget> OnActionPrecast;
        // Action finished
        public CoroutineEvent<MobData, RuntimeAction, Spells.SpellTarget> OnActionPostcast;

        public event CostQueryDelegate OnCostQuery;
        public event CostQueryDelegate OnCostQueryDisplay;
        public CoroutineEvent<Cost, RuntimeAction, MobData> OnCostApply;
        
        /////////////////////////////  Logics  //////////////////////////////

        // TODO: Formalize this !!
        public IEnumerator ReceiveDamage(Consts.DamageHeal_FrontEndInput info, Consts.DamageHeal_Result result)
        {
            if (isDead)
            {
                yield return -1;
            }
            
            // TODO: Trigger pre-computation events
            
            // Compute information and update to info
            float defToUse = 1.0f;
            Consts.DamageHeal_ComputedRates rates = new Consts.DamageHeal_ComputedRates()
            {
                value = 1,
                hit = 1,
                crit = 0,
            };
            
            if (Consts.IsHeal(info))
            {
                rates.value = Mathf.CeilToInt(info.value);
                rates.hit = 1.0f;
                rates.crit = Consts.GetCriticalRate(info.crit, antiCrit, level);
            }
            else
            {
                var defType = Consts.parentType(info.type);
                if(defType == Consts.AllElements.Physical)
                {
                    defToUse = defense;
                }
                else if(defType == Consts.AllElements.Elemental)
                {
                    defToUse = spDefense;
                }
                else
                {
                    defToUse = Consts.GetIdenticalDefense(level);
                }

                rates.value =
                    Consts.GetDamage(info.value, info.source.level, defToUse, level);
                rates.hit = Consts.GetHitRate(info.hit, dodge, level);
                rates.crit = Consts.GetCriticalRate(info.crit, antiCrit, level);
            }
            
            // TODO: Trigger post-rateComputation events
            
            // Roll the dice
            bool isHit = Globals.cc.rng.WithProbability(rates.hit, rates.hit > 0);
            bool isCrit = Globals.cc.rng.WithProbability(rates.crit, rates.crit >= 1.0f);

            // TODO: Trigger post-RNG events

            // TODO: Different multiplier for each type
            float multiplier = isCrit ? 2 : 1;
            if (multiplier > 1)
            {
                rates.value = Mathf.CeilToInt(rates.value * multiplier);
            }

            // Manipulate HP
            int val = 0;
            if (isHit)
            {
                if (info.type == Consts.Elements.Heal)
                {
                    val = Mathf.Clamp(rates.value, 0, maxHealth - health);

                    health += val;

                    if(Globals.cc.animation && mobRenderer != null)
                        yield return new JumpIn(mobRenderer.HealAnimation());
                }
                else
                {
                    val = Mathf.Clamp(rates.value, 0, (int)health);
                    health -= val;
                
                    if(Globals.cc.animation && mobRenderer != null)
                        yield return new JumpIn(mobRenderer.DamageAnimation());
                }
            }

            // TODO: Refine this
            result.source = info.source;
            result.target = this;

            result.isAvoid = !isHit;
            result.isBlock = false;
            result.isCrit = isHit && isCrit;

            result.value = val;
            result.overdeal = Mathf.CeilToInt(rates.value - val);
            result.type = info.type;

            result.flags = info.flags;

            if(info.IsAction)
            {
                result.sourceAction = info.sourceAction;
            }
            else
            {
                result.sourceBuff = info.sourceBuff;
            }

            result.popup = true;
            
            // Do the popup right here before received / dealt events
            Globals.combatTracker.Record(result);
            
            if (isHit)
            {
                if (info.type == Consts.Elements.Heal)
                {
                    yield return new JumpIn(info.source.OnHealDealt?.Invoke(info.source, result));
                    yield return new JumpIn(this.OnHealReceived?.Invoke(this, result));
                }
                else
                {
                    yield return new JumpIn(info.source.OnDamageDealt?.Invoke(info.source, result));
                    yield return new JumpIn(this.OnDamageReceived?.Invoke(this, result));
                }
                
                if(health <= 0 && !isDead)
                {
                    // TODO: Event invoke order; Event termination
                    yield return new JumpIn(this.OnPreDeath?.Invoke(this, result));
                }

                // If we still dead
                if(health <= 0 && !isDead)
                {
                    yield return new JumpIn(this.Killed(result));
                }
            }

            yield break;
        }
        
        private IEnumerator Killed(Consts.DamageHeal_Result info)
        {
            isDead = true;
            yield return new JumpIn(this.OnRealDeath?.Invoke(this, info));

            foreach (var listener in listeners.ToArray())
            {
                RemoveListener(listener);
            }
            
            yield return new JumpIn(SetActive(false));

            if(mobRenderer != null)
                yield return new JumpIn(mobRenderer.Killed(info));

            mobRenderer = null;
            
            // Remove me from world
            Databackend.GetSingleton().ClearMob(Position, gridBody, this, true);
        }
        
        /// <summary>
        /// Should be evaluated immediately. No time delay allowed.
        /// 
        /// When to recalculate:
        /// * Mob initialization
        /// * Wake up (Suspended -> Active)
        /// * Listener added / removed
        /// * Right before Agent / UI
        /// * Before action performed
        /// * After action performed
        /// * Can also be manually triggered from other objects
        /// TODO: Optimize this if performance issue is serious.
        /// </summary>
        [ContextMenu("RefreshStats")]
        public void RecalculateStats()
        {
            baseDescriptor.RecalculateMobBaseStats(this);

            OnBaseStatCalculation?.Invoke(this);

            float healthPercent = (float)health / maxHealth;
            
            baseDescriptor.RecalculateMobBattleStats(this);

            OnStatCalculation?.Invoke(this);

            // Set current health based on previous percentage
            health = Mathf.CeilToInt(maxHealth * healthPercent);

            RefreshActions();
            OnActionStatCalculation?.Invoke(this);

            OnStatCalculationFinish?.Invoke(this);
        }
        
        public void RefreshActions()
        {
            availableActions.Clear();

            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].RecalculateStats(this);
                availableActions.Add(actions[i]);
            }
            
            OnQueryActions?.Invoke(this, availableActions);
        }
    }
}