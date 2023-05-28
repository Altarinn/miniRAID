using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public delegate void MobActionDelegate(MobData mob, Action action, Spells.SpellTarget target);
        
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

        public event MobActionQueryDelegate OnQueryActions;
        
        //// User Interface
        // TODO: ?
        public event MobMenuGUIDelegate OnShowMobMenu;
        
        //// Combat-related

        // Emitted when the mob dealing a damage as the source.
        public CoroutineEvent<MobData, Consts.DamageHeal_FrontEndInput_ByRef> OnDealDmg;
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnDealDamageFinal;

        public CoroutineEvent<MobData, Consts.DamageHeal_FrontEndInput_ByRef> OnDealHeal;
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnDealHealFinal;

        public CoroutineEvent<MobData, Consts.DamageHeal_FrontEndInput_ByRef> OnReceiveDamage;
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnReceiveDamageFinal;

        public CoroutineEvent<MobData, Consts.DamageHeal_FrontEndInput_ByRef> OnReceiveHeal;
        public CoroutineEvent<MobData, Consts.DamageHeal_Result> OnReceiveHealFinal;

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
        
        public IEnumerator ReceiveDamage(Consts.DamageHeal_FrontEndInput info, Consts.DamageHeal_Result result)
        {
            if (isDead)
            {
                yield return -1;
            }
            
            int val;

            // TODO: Scale crit% with level.
            bool isCrit = UnityEngine.Random.Range(0, 100.0f) < info.crit;
            float multiplier = isCrit ? 2 : 1;

            //Globals.debugMessage.Instance.Message(info.crit.ToString());
            info.value *= multiplier;

            if (info.type == Consts.Elements.Heal)
            {
                val = Mathf.CeilToInt(Mathf.Min(info.value, maxHealth - health));

                health += val;

                if(Globals.cc.animation && mobRenderer != null)
                    yield return new JumpIn(mobRenderer.HealAnimation());
            }
            else
            {
                // Defense
                var defType = Consts.parentType(info.type);
                float def = 0;
                if(defType == Consts.AllElements.Physical)
                {
                    def = defense;
                }
                else if(defType == Consts.AllElements.Elemental)
                {
                    def = spDefense;
                }
                val = Mathf.CeilToInt(Mathf.Max(0.01f, info.value * (1.0f - def * 0.01f)));

                val = Mathf.Min(val, (int)health);
                health -= val;
                
                if(Globals.cc.animation && mobRenderer != null)
                    yield return new JumpIn(mobRenderer.DamageAnimation());
            }

            // TODO: Refine this
            result.source = info.source;
            result.target = this;

            result.isAvoid = false;
            result.isBlock = false;
            result.isCrit = false;

            result.value = val;
            result.overdeal = Mathf.CeilToInt(info.value - val);
            result.type = info.type;

            if(info.IsAction)
            {
                result.sourceAction = info.sourceAction;
            }
            else
            {
                result.sourceBuff = info.sourceBuff;
            }

            result.popup = true;

            yield return new JumpIn(this.OnReceiveDamageFinal?.Invoke(this, result));

            if(health <= 0)
            {
                // TODO: Event invoke order; Event termination
                yield return new JumpIn(this.OnPreDeath?.Invoke(this, result));
            }

            // If we still dead
            if(health <= 0)
            {
                yield return new JumpIn(this.Killed(result));
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

            if(mobRenderer != null)
                yield return new JumpIn(mobRenderer.Killed(info));

            yield return new JumpIn(SetActive(false));
            
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
            baseStats.VIT = dNumber.CreateComposite(baseStatsFundamental.VIT, "mobbase");
            baseStats.STR = dNumber.CreateComposite(baseStatsFundamental.STR, "mobbase");
            baseStats.MAG = dNumber.CreateComposite(baseStatsFundamental.MAG, "mobbase");
            baseStats.INT = dNumber.CreateComposite(baseStatsFundamental.INT, "mobbase");
            baseStats.DEX = dNumber.CreateComposite(baseStatsFundamental.DEX, "mobbase");
            baseStats.TEC = dNumber.CreateComposite(baseStatsFundamental.TEC, "mobbase");

            OnBaseStatCalculation?.Invoke(this);

            battleStats = new Consts.BattleStats();

            // TODO: Fill Battle stats with Basic calculations
            float healthPercent = (float)health / maxHealth;
            maxHealth = dNumber.CreateComposite(level * 2 + VIT * 3, "mobbase");

            //data.defense = dNumber.CreateComposite(VIT * 2 + STR, "mobbase");
            //data.spDefense = dNumber.CreateComposite(MAG * 2 + INT, "mobbase");

            attackPower = dNumber.CreateComposite(STR, "mobbase");
            spellPower = dNumber.CreateComposite(INT, "mobbase");
            
            // DEBUG ONLY
            healPower = dNumber.CreateComposite(MAG, "mobbase");
            // DEBUG ONLY ENDS

            hitAcc = dNumber.CreateComposite(TEC, "mobbase");
            crit = dNumber.CreateComposite(TEC, "mobbase");
            dodge = dNumber.CreateComposite(DEX, "mobbase");
            antiCrit = dNumber.CreateComposite(DEX, "mobbase");

            aggroMul = dNumber.CreateComposite(1.0, "mobbase");

            OnStatCalculation?.Invoke(this);

            // Set current health based on pervious percentage
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