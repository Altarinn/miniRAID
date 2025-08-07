using System.Collections;
using System.Collections.Generic;
using System.Linq;
using miniRAID;
using miniRAID.Spells;
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
        public delegate bool CostCheckBooleanDelegate(Cost cost, RuntimeAction ract, MobData mob);
        // WTF??? How about mana-draining attacks?
        // Cost = Damage ?

        // TODO: Think about this signature
        public delegate void MobActionQueryDelegate(MobData mob, HashSet<RuntimeAction> actions);

        public delegate IEnumerator MobDataCoroutineDelegate(MobData mob);
        public delegate IEnumerator MobMovedCoroutineDelegate(MobData mob, Vector3Int destination);

        public delegate IEnumerator MobDamageHealPreCalcCoroutineDelegate(MobData mob, Consts.DamageHeal_FrontEndInput input);
        public delegate IEnumerator MobDamageHealResultCoroutineDelegate(MobData mob, Consts.DamageHeal_Result input);

        public delegate IEnumerator MobDamageHealBeforeApplicationCoroutineDelegate(MobData mob,
            Consts.DamageHeal_FrontEndInput input, Consts.DamageHeal_ComputedRates rates);

        public delegate IEnumerator MobActionWithTargetCoroutineDelegate(MobData mob, RuntimeAction action,
            Spells.SpellTarget target);

        public delegate IEnumerator MobOnApplyCostCoroutineDelegate(Cost cost, RuntimeAction action, MobData mob);
        
        
        /////////////////////////////  Events  //////////////////////////////
        /// TODO: Invoke order ...... (Has priority now but nobody use)

        //// Timing

        // Before status re-calculation; 
        //public event MobArgumentDelegate OnEarlyWakeup;

        // Emitted when the mob has its active state turned from "slept / 待机" to "awake / 可行动", after resetted all related parameters.
        public MobEvent<MobDataCoroutineDelegate> OnWakeup = new();

        // Used for agents, after the regular wakeup process
        public MobEvent<MobDataCoroutineDelegate> OnAgentWakeUp = new();
        
        // Used for auto-attack agents
        public MobEvent<MobDataCoroutineDelegate> OnAutoAttackAgentWakeUp = new();

        // Emitted when the mob has entered a new turn ("PHASE" for this mob has started)
        public MobEvent<MobDataCoroutineDelegate> OnNextTurn = new();
        
        // Emitted when the mob has entered a new turn ("PHASE" for this mob has started)
        public MobEvent<MobDataCoroutineDelegate> OnRecoveryStage = new();
        
        //// Status calculation
        
        public MobEvent<MobArgumentDelegate> OnInitialized = new();
        public MobEvent<MobArgumentDelegate> OnBaseStatCalculation = new();
        public MobEvent<MobArgumentDelegate> OnStatCalculation = new();
        public MobEvent<MobArgumentDelegate> OnActionStatCalculation = new();
        public MobEvent<MobArgumentDelegate> OnStatCalculationFinish = new();
        
        // Parameters: MobData, previous position
        public MobEvent<MobMovedCoroutineDelegate> OnMobMoved = new();

        public MobEvent<MobActionQueryDelegate> OnQueryActions = new();

        //// User Interface
        // TODO: ?
        public MobEvent<MobMenuGUIDelegate> OnShowMobMenu = new();
        
        //// Combat-related

        /// <summary>
        /// Emitted when the mob dealing a damage as the source.
        /// This event happens before the damage is dealt. Damage can be modified.
        /// This event also happens before rolling the RNG for hit/dodge and critical strikes.
        /// As they are represented as corresponding probabilities (values) instead of rolled results.
        /// </summary>
        public MobEvent<MobDamageHealPreCalcCoroutineDelegate> OnDealDmg = new();
        // public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnDealDamageFinal;
        
        /// <summary>
        /// Emitted when the mob has finished dealt a damage as the source.
        /// </summary>
        public MobEvent<MobDamageHealResultCoroutineDelegate> OnDamageDealt = new();

        public MobEvent<MobDamageHealPreCalcCoroutineDelegate> OnDealHeal = new();
        // public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnDealHealFinal;
        public MobEvent<MobDamageHealResultCoroutineDelegate> OnHealDealt = new();

        public MobEvent<MobDamageHealPreCalcCoroutineDelegate> OnReceiveDamage = new();
        public MobEvent<MobDamageHealBeforeApplicationCoroutineDelegate> OnBeforeDamageApplied = new();
        // public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnReceiveDamageFinal;
        public MobEvent<MobDamageHealResultCoroutineDelegate> OnDamageReceived = new();

        public MobEvent<MobDamageHealPreCalcCoroutineDelegate> OnReceiveHeal = new();
        public MobEvent<MobDamageHealBeforeApplicationCoroutineDelegate> OnBeforeHealApplied = new();
        // public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnReceiveHealFinal;
        public MobEvent<MobDamageHealResultCoroutineDelegate> OnHealReceived = new();

        public MobEvent<MobDamageHealResultCoroutineDelegate> OnKill = new(); // TODO: ByRef?
        public MobEvent<MobDamageHealResultCoroutineDelegate> OnPreDeath = new(); // TODO: ByRef?
        public MobEvent<MobDamageHealResultCoroutineDelegate> OnRealDeath = new();

        // When we still don't know if the action can be performed or not, but targets already pick'd
        public MobEvent<MobActionWithTargetCoroutineDelegate> OnActionChosen = new();
        // Action ready, confirmed okay to be performed, but not performed yet
        public MobEvent<MobActionWithTargetCoroutineDelegate> OnActionPrecast = new();
        // Action finished
        public MobEvent<MobActionWithTargetCoroutineDelegate> OnActionPostcast = new();

        public MobEvent<CostQueryDelegate> OnModifyCost = new();
        
        /// <summary>
        /// Emits when the mob is checking a cost to see if it meets the requirements.
        /// Costs include mana, HP, etc.
        /// The check passes if any of the delegates returns true.
        /// Therefore, make sure you return false if 1) the cost is not applicable, or 2) the requirement is not met.
        /// </summary>
        public MobEvent<CostCheckBooleanDelegate> OnCheckCost = new();
        public MobEvent<MobOnApplyCostCoroutineDelegate> OnApplyCost = new();

        public MobEvent<CostQueryDelegate> OnCostQueryDisplay = new();

        /// <summary>
        /// Emitted when the mob is being selected in the UI.
        /// Perhaps always comes with a valid MobRenderer.
        /// </summary>
        public MobEvent<MobArgumentDelegate> OnMobSelectedInUI = new();
        public MobEvent<MobArgumentDelegate> OnMobDeselectedInUI = new();
        
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
            
            // Trigger Before Applied events
            if (Consts.IsHeal(info))
            {
                yield return new JumpIn(OnBeforeHealApplied?.InvokeCoroutine(this, info, rates));
            }
            else
            {
                yield return new JumpIn(OnBeforeDamageApplied?.InvokeCoroutine(this, info, rates));
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
                    yield return new JumpIn(info.source.OnHealDealt?.InvokeCoroutine(info.source, result));
                    yield return new JumpIn(this.OnHealReceived?.InvokeCoroutine(this, result));
                }
                else
                {
                    yield return new JumpIn(info.source.OnDamageDealt?.InvokeCoroutine(info.source, result));
                    yield return new JumpIn(this.OnDamageReceived?.InvokeCoroutine(this, result));
                }
                
                if(health <= 0 && !isDead)
                {
                    // TODO: Event invoke order; Event termination
                    yield return new JumpIn(this.OnPreDeath?.InvokeCoroutine(this, result));
                }

                // If we still dead
                if(health <= 0 && !isDead)
                {
                    yield return new JumpIn(this.Killed(result));
                }
            }

            yield break;
        }

        public IEnumerator Kill()
        {
            Consts.DamageHeal_Result dummyInfo = null;
            yield return new JumpIn(Killed(dummyInfo));
        }
        
        private IEnumerator Killed(Consts.DamageHeal_Result info)
        {
            isDead = true;
            initialized = false;
            yield return new JumpIn(this.OnRealDeath?.InvokeCoroutine(this, info));

            foreach (var listener in listeners.ToArray())
            {
                RemoveListener(listener);
            }

            bool shouldDestroy = unitGroup != Consts.UnitGroup.Player;
            
            yield return new JumpIn(SetActive(false));

            if(mobRenderer != null)
                yield return new JumpIn(mobRenderer.Killed(info, shouldDestroy));

            if (shouldDestroy)
            {
                renderer = null;
            
                // Remove me from world
                Databackend.GetSingleton().ClearMob(this, true);
            }
        }

        /// <summary>
        /// Revives the mob. If the mob is not dead, nothing happens.
        /// </summary>
        /// <param name="info">The healing info used to revive the mob</param>
        /// <returns>Coroutine</returns>
        public IEnumerator Revive(Consts.DamageHeal_FrontEndInput info)
        {
            if (!isDead)
            {
                yield break;
            }
            
            isDead = false;
            Init();
            
            // Give basic health info
            health = 1;
            RecalculateStats();

            // Apply healing
            Consts.DamageHeal_Result result = new Consts.DamageHeal_Result();
            yield return new JumpIn(ReceiveDamage(info, result));
        }

        // TODO: Remove this
        public IEnumerator Cheat<TSpellTarget>(RuntimeAction<TSpellTarget> ract) where TSpellTarget : SpellTarget
        {
            Consts.DamageHeal_FrontEndInput info = new Consts.DamageHeal_FrontEndInput()
            {
                crit = 0,
                flags = Consts.DamageHealFlags.Indirect,
                hit = 1000000,
                popup = true,
                source = this,
                type = Consts.Elements.Heal,
                value = maxHealth * 0.2f,
                sourceAction = ract
            };

            if (isDead)
            {
                yield return new JumpIn(Revive(info));
            }
            else
            {
                Consts.DamageHeal_Result result = new Consts.DamageHeal_Result();
                yield return new JumpIn(ReceiveDamage(info, result));
            }
        }

        public void SetSkipNextAutoAttack()
        {
            skipAutoAttack = true;
        }
        
        public void ResetSkipNextAutoAttack()
        {
            skipAutoAttack = false;
        }

        public IEnumerator AutoAttack()
        {
            if(!canAutoAttack)
                yield break;
            
            yield return new JumpIn(this.OnAutoAttackAgentWakeUp?.InvokeCoroutine(this));
            
            RecalculateStats();
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

            OnBaseStatCalculation?.InvokeInstant(this);

            float healthPercent = (float)health / maxHealth;
            
            baseDescriptor.RecalculateMobBattleStats(this);

            OnStatCalculation?.InvokeInstant(this);

            // Set current health based on previous percentage
            health = Mathf.Clamp(Mathf.CeilToInt(maxHealth * healthPercent), 0, maxHealth);

            RefreshActions();
            OnActionStatCalculation?.InvokeInstant(this);

            OnStatCalculationFinish?.InvokeInstant(this);
        }
        
        public void RefreshActions()
        {
            availableActions.Clear();

            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].RecalculateStats(this);
                if (actions[i] == mainWeapon?.GetRegularAttackSpell())
                {
                    if (!IsInGCD(GCDGroup.RegularAttack))
                    {
                        availableActions.Add(actions[i]);
                    }
                }
                else
                {
                    availableActions.Add(actions[i]);
                }
            }
            
            OnQueryActions?.InvokeInstant(this, availableActions);
        }

        public void SelectedInUI()
        {
            OnMobSelectedInUI?.InvokeInstant(this);
        }
        
        public void DeselectedInUI()
        {
            OnMobDeselectedInUI?.InvokeInstant(this);
        }
    }
}