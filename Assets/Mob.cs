using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using System.Linq;

using NuclearBand;

namespace miniRAID
{
    [XLua.LuaCallCSharp]
    [ParameterDefaultName("mob")]
    public partial class Mob : MonoBehaviour
    {
        // A bunch of grids occpied by this mob (for 1x1 mobs, only 1 grid; 2x2 -> 4 grids, etc.)
        // TODO: seperate GridBody into a single MonoBehaviour
        public GridShape gridBody;

        Databackend backend;
        public bool isBoss;
        public bool enemyDebug = false;
        public MobData data;

        public Animator animator;

        // Delegates
        public delegate void MobArgumentDelegate(Mob mob);
        public delegate void MobDHInputArgumentDelegate(Mob mob, ref Consts.DamageHeal_FrontEndInput info);
        public delegate void MobDHResultArgumentDelegate(Mob mob, Consts.DamageHeal_Result info);
        public delegate void MobDHResultConfirmedArgumentDelegate(Mob mob, Consts.DamageHeal_Result info);
        public delegate void MobMenuGUIDelegate(Mob mob, UI.UnitMenu state, UI.UIMenu_UIContainer menu);

        public delegate void CostQueryDelegate(Cost cost, RuntimeAction ract, Mob mob);
            // WTF??? How about mana-draining attacks?
            // Cost = Damage ?

        // TODO: Think about this signature
        public delegate void MobActionQueryDelegate(Mob mob, HashSet<RuntimeAction> actions);
        public delegate void MobActionDelegate(Mob mob, Action action, Spells.SpellTarget target);

        ///////////////////////////// Events /////////////////////////////
        /// TODO: Invoke order ......

        //// Timing

        // Before status re-calculation; 
        //public event MobArgumentDelegate OnEarlyWakeup;

        // Emitted when the mob has its active state turned from "slept / 待机" to "awake / 可行动", after resetted all related parameters.
        public CoroutineEvent<Mob> OnWakeup;

        // Used for agents, after the regular wakeup process
        public CoroutineEvent<Mob> OnAgentWakeUp;

        // Emitted when the mob has entered a new turn ("PHASE" for this mob has started)
        public CoroutineEvent<Mob> OnNextTurn;

        //// Status calculation

        public event MobArgumentDelegate OnBaseStatCalculation;
        public event MobArgumentDelegate OnStatCalculation;
        public event MobArgumentDelegate OnActionStatCalculation;
        public event MobArgumentDelegate OnStatCalculationFinish;

        public event MobActionQueryDelegate OnQueryActions;

        //// User Interface

        public event MobMenuGUIDelegate OnShowMobMenu;

        //// Combat-related

        // Emitted when the mob dealing a damage as the source.
        public event MobDHInputArgumentDelegate OnDealDmg;
        public event MobDHResultArgumentDelegate OnDealDamageFinal;

        public event MobDHInputArgumentDelegate OnDealHeal;
        public event MobDHResultArgumentDelegate OnDealHealFinal;

        public event MobDHInputArgumentDelegate OnReceiveDamage;
        public event MobDHResultArgumentDelegate OnReceiveDamageFinal;

        public event MobDHInputArgumentDelegate OnReceiveHeal;
        public event MobDHResultArgumentDelegate OnReceiveHealFinal;

        public event MobDHResultArgumentDelegate OnKill;
        public event MobDHResultArgumentDelegate OnPreDeath;
        public event MobDHResultConfirmedArgumentDelegate OnRealDeath;

        // When we still don't know if the action can be performed or not, but targets already pick'd
        public CoroutineEvent<Mob, RuntimeAction, Spells.SpellTarget> OnActionChosen;
        // Action ready, confirmed okay to be performed, but not performed yet
        public CoroutineEvent<Mob, RuntimeAction, Spells.SpellTarget> OnActionPrecast;
        // Action finished
        public CoroutineEvent<Mob, RuntimeAction, Spells.SpellTarget> OnActionPostcast;

        public event CostQueryDelegate OnCostQuery;
        public event CostQueryDelegate OnCostQueryDisplay;
        public CoroutineEvent<Cost, RuntimeAction, Mob> OnCostApply;

        #region Stats

        public int VIT => (int)data.baseStats.VIT.Value;
        public int STR => (int)data.baseStats.STR.Value;
        public int MAG => (int)data.baseStats.MAG.Value;
        public int INT => (int)data.baseStats.INT.Value;
        public int DEX => (int)data.baseStats.DEX.Value;
        public int TEC => (int)data.baseStats.TEC.Value;

        public int AttackPower => (int)data.attackPower.Value;
        public int SpellPower => (int)data.spellPower.Value;
        public int HealPower => (int)data.healPower.Value;
        public int BuffPower => (int)data.buffPower.Value;

        public int Def => (int)data.defense.Value;
        public int SpDef => (int)data.spDefense.Value;

        public float AggroMul => (float)data.aggroMul.Value;

        #endregion


        //public MobDHInputArgumentDelegate

        // Use this for initialization
        void Start()
        {
            gridBody = new GridShape();
            animator = GetComponent<Animator>();

            backend = Databackend.GetSingleton();
            UpdateGridPos();

            // Find my bodies
            foreach (MobGridProxy proxy in GetComponentsInChildren<MobGridProxy>())
            {
                gridBody.AddGrid(Databackend.GetSingleton().GetGridPos(proxy.transform.position) - data.Position);
            }

            data.parentMob = this;
            data.nickname = this.name;
            data.Init(this);

            backend.SetMob(data.Position, gridBody, this);

            // Initial stats calculation
            RecalculateStats();

            if(isBoss)
            {
                Globals.ui.Instance.combatView.BindAsBoss(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        void UpdateGridPos()
        {
            data.Position = backend.GetGridPos(transform.position);
        }

        public delegate void GeneralCallback();

        public void MoveTo(Vector2Int newPos, GeneralCallback callback = null, GridPath path = null)
        {
            // TODO: check if able to move

            // Start move and wait for complete
            StartCoroutine(MoveToCorotine(newPos, callback, path));
        }

        public bool UseActionPoint(float v)
        {
            if(data.actionPoints >= v)
            {
                data._SetActionPoints(data.actionPoints - v);
                return true;
            }
            return false;
        }

        public IEnumerator MoveToCorotine(Vector2Int targetPos, GeneralCallback callback, GridPath path)
        {
            Vector3 targetPos_real = new Vector3(targetPos.x + 0.5f, targetPos.y + 0.5f, transform.position.z);

            // TODO: implement path for field effects (move w.r.t. the path & tell backend that we reached a intermediate point)
            // Move until reached target
            while ((transform.position - targetPos_real).magnitude >= 1e-3)
            {
                // Move a little bit
                transform.position = Vector3.MoveTowards(transform.position, targetPos_real, 5.0f * Time.deltaTime);

                // Wait 1 frame before next movement
                yield return null;
            }

            // Tell backend that we finished the movement

            // TODO: change to use path
            UseActionPoint((Mathf.Abs(targetPos.x - data.Position.x) + Mathf.Abs(targetPos.y - data.Position.y)) / 2.0f);

            transform.position = targetPos_real;
            UpdateGridPos();

            callback?.Invoke();
        }

        public IEnumerator CoroutineTest()
        {
            Debug.Log("01MobTest");
            yield return new WaitForSeconds(1);
            Debug.Log("02MobTest - Finish");
        }

        public IEnumerator DoActionWithDefaultCosts(
            RuntimeAction raction,
            Spells.SpellTarget target)
        {
            List<Cost> cost = raction.data.costs.Select(pair =>
                new Cost(dNumber.CreateComposite(pair.Value.Eval((this, target))), pair.Key)).ToList();
            yield return new JumpIn(DoAction(raction, target, cost));
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

            yield return new JumpIn(ActionBegin(raction, target));
            yield return new JumpIn(raction.Activate(this, target));
            yield return new JumpIn(ActionDone(raction, target));
        }

        public bool CheckCost(Cost cost, RuntimeAction ract)
        {
            // TODO: Dummy action
            var mCost = GetModifiedCost(cost, ract);

            switch(mCost.type)
            {
                case Cost.Type.AP:
                    return cost.value <= data.actionPoints;
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

        public void GetResource(Cost cost)
        {
            switch (cost.type)
            {
                case Cost.Type.AP:
                    Debug.LogWarning("Please implement GetResource for AP.");
                    break;
                case Cost.Type.Mana:
                    Debug.LogWarning("Please implement GetResource for Mana.");
                    break;
            }
            
            return;
        }

        internal bool CheckCalculatedActionCostBounds(RuntimeAction action)
        {
            return !action.costBounds.Select(cost => CheckCost(cost.Item1, action)).Any(x => !x);
        }

        //public void DoAction(
        //    Action action, Spells.SpellTarget target,
        //    Action.OnActionCorotinePreExecute beforeAnimation = null,
        //    Action.OnActionCorotineFinished afterAnimation = null,
        //    Action.OnActionPerformed callback = null)
        //{
        //    RecalculateStats();
        //    ActionPrecheck(action, target);
        //    if (action.DoCost(this, target))
        //    {
        //        beforeAnimation?.Invoke();
        //        action.Activate(this, target, () => { afterAnimation?.Invoke(); RecalculateStats(); }, true);
        //        callback?.Invoke(action, this);
        //    }
        //}

        //public void DoActionFree(
        //    Action action, Spells.SpellTarget target,
        //    Action.OnActionCorotinePreExecute beforeAnimation = null,
        //    Action.OnActionCorotineFinished afterAnimation = null,
        //    Action.OnActionPerformed callback = null)
        //{
        //    RecalculateStats();
        //    ActionPrecheck(action, target);
        //    beforeAnimation?.Invoke();
        //    action.Activate(this, target, () => { afterAnimation?.Invoke(); RecalculateStats(); }, true);
        //    callback?.Invoke(action, this);
        //}

        // TODO: multi-target / ground-target / etc.
        public void RegularAttack(Spells.SpellTarget target, Action.OnActionCorotineFinished callback = null, bool doCost = true)
        {
            // TODO: Change Spell to ActionDataSO ...

            throw new System.NotImplementedException();
            if(doCost)
            {
                //DoAction(data.mainWeapon.GetRegularAttackSpell(), target);
            }
            //else
            //{
            //    DoActionFree(data.mainWeapon.GetRegularAttackSpell(), target, null, callback);
            //}
        }

        public IEnumerator ReceiveDamage(Consts.DamageHeal_FrontEndInput info, Consts.DamageHeal_Result result)
        {
            int val;

            // TODO: Scale crit% with level.
            bool isCrit = UnityEngine.Random.Range(0, 100.0f) < info.crit;
            float multiplier = isCrit ? 2 : 1;

            //Globals.debugMessage.Instance.Message(info.crit.ToString());
            info.value *= multiplier;

            if (info.type == Consts.Elements.Heal)
            {
                val = Mathf.CeilToInt(Mathf.Min(info.value, data.maxHealth - data.health));

                data.health += val;

                // FIXME: Test animation
                GetComponentInChildren<SpriteRenderer>().color = Color.green;
                yield return new WaitForSeconds(.1f);
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
            else
            {
                // Defense
                var defType = Consts.parentType(info.type);
                float def = 0;
                if(defType == Consts.AllElements.Physical)
                {
                    def = Def;
                }
                else if(defType == Consts.AllElements.Elemental)
                {
                    def = SpDef;
                }
                val = Mathf.CeilToInt(Mathf.Max(0.01f, info.value * (1.0f - def * 0.01f)));

                val = Mathf.Min(val, (int)data.health);
                data.health -= val;

                GetComponentInChildren<SpriteRenderer>().color = Color.red;
                yield return new WaitForSeconds(.1f);
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
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

            this.OnReceiveDamageFinal?.Invoke(this, result);

            if(data.health <= 0)
            {
                // TODO: Event invoke order; Event termination
                this.OnPreDeath?.Invoke(this, result);
            }

            // If we still dead
            if(data.health <= 0)
            {
                this.Killed(result);
            }

            yield break;
        }

        private void Killed(Consts.DamageHeal_Result info)
        {
            data.isDead = true;
            this.OnRealDeath?.Invoke(this, info);

            foreach (var listener in data.listeners.ToArray())
            {
                data.RemoveListener(listener);
            }

            Globals.debugMessage.Instance.Message($"{info.source?.name} 的 {info.sourceAction?.data.ActionName} 击杀了 {name} !");

            // Remove me from world
            SetActive(false);
            backend.ClearMob(data.Position, gridBody, this, true);

            // TODO: Proper logic to destroy
            //Destroy(this.gameObject);
        }

        public bool ReceiveBuff(Buff.BuffSO buff, Mob source)
        {
            AddListener(buff.Wrap(source.data));
            return true;
        }

        public bool ReceiveBuff(Buff.Buff buff)
        {
            // TODO: Check if buff duplicated
            AddListener(buff);
            return true;
        }

        public bool RemoveListener(MobListener listener)
        {
            data.RemoveListener(listener);
            return true;
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
            data.baseStats.VIT = dNumber.CreateComposite(data.baseStatsFundamental.VIT, "mobbase");
            data.baseStats.STR = dNumber.CreateComposite(data.baseStatsFundamental.STR, "mobbase");
            data.baseStats.MAG = dNumber.CreateComposite(data.baseStatsFundamental.MAG, "mobbase");
            data.baseStats.INT = dNumber.CreateComposite(data.baseStatsFundamental.INT, "mobbase");
            data.baseStats.DEX = dNumber.CreateComposite(data.baseStatsFundamental.DEX, "mobbase");
            data.baseStats.TEC = dNumber.CreateComposite(data.baseStatsFundamental.TEC, "mobbase");

            OnBaseStatCalculation?.Invoke(this);

            data.battleStats = new Consts.BattleStats();

            // TODO: Fill Battle stats with Basic calculations
            float healthPercent = (float)data.health / data.maxHealth;
            data.maxHealth = dNumber.CreateComposite(data.level * 2 + VIT * 3, "mobbase");

            //data.defense = dNumber.CreateComposite(VIT * 2 + STR, "mobbase");
            //data.spDefense = dNumber.CreateComposite(MAG * 2 + INT, "mobbase");

            data.attackPower = dNumber.CreateComposite(STR, "mobbase");
            data.spellPower = dNumber.CreateComposite(INT, "mobbase");
            
            // DEBUG ONLY
            data.healPower = dNumber.CreateComposite(MAG, "mobbase");
            // DEBUG ONLY ENDS

            data.hitAcc = dNumber.CreateComposite(TEC, "mobbase");
            data.crit = dNumber.CreateComposite(TEC, "mobbase");
            data.dodge = dNumber.CreateComposite(DEX, "mobbase");
            data.antiCrit = dNumber.CreateComposite(DEX, "mobbase");

            data.aggroMul = dNumber.CreateComposite(1.0, "mobbase");

            OnStatCalculation?.Invoke(this);

            // Set current health based on pervious percentage
            data.health = Mathf.CeilToInt(data.maxHealth * healthPercent);

            RefreshActions();
            OnActionStatCalculation?.Invoke(this);

            OnStatCalculationFinish?.Invoke(this);
        }

        private void RefreshActions()
        {
            data.RefreshActions();

            OnQueryActions?.Invoke(this, data.availableActions);
        }

        #region Actions

        public RuntimeAction AddAction(ActionDataSO actSO)
        {
            var ract = data.AddAction(actSO);
            RecalculateStats();
            return ract;
        }

        /// <summary>
        /// Get the first available action with action name.
        /// </summary>
        /// <param name="ActionName"></param>
        /// <returns></returns>
        public RuntimeAction GetAction(string ActionName)
        {
            foreach (var act in data.availableActions)
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
            foreach (var act in data.availableActions)
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
            foreach (var act in data.availableActions)
            {
                if (act is T)
                {
                    return act;
                }
            }

            return null;
        }

        #endregion

        #region Combat Helpers

        //public void DamageTo(RuntimeAction ract, Mob target, float value, Consts.Elements type, )
        //{
        //    input.source = this;
        //    backend.DealDmgHeal(target, input);
        //}

        // TODO: Good damageTo in Lua
        public IEnumerator DamageTo(Mob target, Consts.DamageHeal_FrontEndInput input)
        {
            input.source = this;
            yield return new JumpIn(backend.DealDmgHeal(target, input));

            yield break;
        }

        // TODO: Change to Addressables.
        // https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/LoadingAddressableAssets.html
        // https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/SynchronousAddressables.html
        //public IEnumerator BuffTo(Mob target, string buffPath)
        //{
        //    Buff.BuffSO buff = SODatabase.GetModel<Buff.BuffSO>($"Buffs/{buffPath}");

        //    // TODO: Buff power
        //    //buff

        //    target.ReceiveBuff(buff, this);

        //    yield break;
        //}

        //public Buff.Buff GetBuffObject(string buffPath)
        //{
        //    return (Buff.Buff)SODatabase.GetModel<Buff.BuffSO>($"Buffs/{buffPath}").Wrap(this.data);
        //}

        //public Buff.GridEffect GetGridFxObject(string fxPath, Vector3 position)
        //{
        //    return (Buff.GridEffect)SODatabase.GetModel<Buff.GridEffectSO>($"Buffs/GridFx/{fxPath}").WrapFx(this.data, position);
        //}

        #endregion

        public void AddListener(MobListenerSO listener)
        {
            data.AddListener(listener);
        }

        public void AddListener(MobListener listener)
        {
            data.AddListener(listener);
        }

        public void ShowMenu(UI.UnitMenu menu, UI.UIMenu_UIContainer container)
        {
            OnShowMobMenu?.Invoke(this, menu, container);
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

        public bool isActive
        {
            get
            {
                return data.isActive && (!data.isDead);
            }
        }

        public IEnumerator SetActive(bool value)
        {
            if(data.isActive == value) { yield break; }

            if (value)
            {
                data.isActive = true;

                data.OnWakeUp(this);
                RecalculateStats(); // Do we need it here?
                yield return new JumpIn(OnWakeup?.Invoke(this));

                RecalculateStats();
                yield return new JumpIn(OnAgentWakeUp?.Invoke(this));
            }
            else
            {
                // FIXME: Test animation
                GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
                yield return new WaitForSeconds(.25f);
                GetComponentInChildren<SpriteRenderer>().color = Color.white;

                data.isActive = false;
            }
        }

        public IEnumerator WaitForAnimation(string animationState)
        {
            if(animator == null) { yield break; }

            int hash = Animator.StringToHash($"Base Layer.{animationState}");
            animator.Play(hash);

            //Wait until we enter the current state
            while (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != hash)
            {
                yield return null;
            }

            float counter = 0;
            float waitTime = animator.GetCurrentAnimatorStateInfo(0).length;

            //Now, Wait until the current state is done playing
            while (counter < (waitTime))
            {
                counter += Time.deltaTime;
                yield return null;
            }
        }

        public IEnumerator TryAutoEndTurn()
        {
            if(data.actionPoints <= 0)
            {
                yield return new JumpIn(SetActive(false));
            }
        }

        public IEnumerator _OnNextTurn()
        {
            yield return new JumpIn(OnNextTurn?.Invoke(this));
        }
    }
}
