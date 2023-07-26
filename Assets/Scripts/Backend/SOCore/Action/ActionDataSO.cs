using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Weapon;
using UnityEngine.Localization;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace miniRAID
{
    [LuaCallCSharp]
    [ParameterDefaultName("s")]
    public class GeneralCombatData
    {
        public dNumber power, auxPower;
        public GridShape shape;
        public RuntimeAction ract;

        public GameObject[] gameObjects;
        public Sprite[] sprites;

        public GeneralCombatData() { }

        public GeneralCombatData(float p)
        {
            power = (dNumber)p;
            auxPower = (dNumber)0;
            shape = null;
        }

        public GeneralCombatData(dNumber p)
        {
            power = p;
            auxPower = (dNumber)0;
            shape = null;
        }
    }

    public class PowerGetter
    {
        public enum PowerGetterType
        {
            STATIC,
            AttackPower,
            SpellPower,
            HealPower,
            BuffPower,
            DYNAMIC,
        }

        public PowerGetterType powerType;
        public LeveledStats<float> powerFactor;
        public System.Func<MobData, float> dynamicGetter;

        public PowerGetter(float val)
        {
            powerType = PowerGetterType.AttackPower;
            powerFactor = new LeveledStats<float>();
        }

        public float Eval(int level, MobData param)
        {
            switch (powerType)
            {
                case PowerGetterType.STATIC:
                    return powerFactor.Eval(level);
                    break;
                    
                case PowerGetterType.DYNAMIC:
                    return dynamicGetter(param);
                    break;
                
                case PowerGetterType.AttackPower:
                    return param.attackPower * powerFactor.Eval(level);
                    break;
                
                case PowerGetterType.SpellPower:
                    return param.spellPower * powerFactor.Eval(level);
                    break;
                
                case PowerGetterType.HealPower:
                    return param.healPower * powerFactor.Eval(level);
                    break;
                
                case PowerGetterType.BuffPower:
                    return param.buffPower * powerFactor.Eval(level);
                    break;
            }

            return 0;
        }
    }

    public class ColoredBoxAttribute : Attribute
    {
        public Color color;

        public ColoredBoxAttribute(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out color);
        }
    }

    /* TODO: Update doc from ActionDataOnPerformSO to ActionDataSO
     * The class for actions (data).
     * To provide a custom implementation (so you can use it in your actions), please
     * implement `OnPerform()`.
     *
     * Your implementation must be pure function, otherwise the behaviour is undefined.
     * Those ScriptableObjects won't be copied / instantiated during runtime, and only 1 instance (per created assets) will be kept in memory.
     *
     * Pure function means that your function does not have any side effects.
     * i.e., you cannot modify some variables that is outside of this function's scope.
     * To keep track of some external state, consider apply buffs to the source mob and query for that buff each time.
     */
    [LuaCallCSharp]
    [CreateAssetMenu(fileName = "ActionData.asset", menuName = "ActionDataSO", order = 0)]
    public class ActionDataSO : CustomIconScriptableObject
    {
        [Title("Basic info")]
        public LocalizedString ActionNameKey;

        public string ActionName => Globals.localizer.L(ActionNameKey) ?? "BAD_STRING";

        [FormerlySerializedAs("MaxLevel")] public int maxLevel;

        // [TextArea(1, 25)]
        public LocalizedString DescriptionKey;
        public string Description => Globals.localizer.L(DescriptionKey) ?? "BAD_STRING";

        [Title("Flags")] public Consts.ActionFlags flags;

        // TODO: Boolean arrays
        // public List<string> Tags;
        [Title("Power stats")]
        public PowerGetter power;
        public PowerGetter auxPower;
        // public LeveledStats<float> test;

        // Cost related
        [Title("Costs", horizontalLine: true, bold: true)]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine)]
        public Dictionary<
            Cost.Type,
            LuaBoundedGetter<(MobData, Spells.SpellTarget), MobData, double>> costs = new();
        //public LuaGetter<(Mob, Spells.SpellTarget), GCDGroup> gcdGroup = GCDGroup.Common;

        public LuaGetter<MobData, bool> isActivelyUsed = true;

        [Title("Requester & Validation", horizontalLine: true, bold: true)]
        [Sirenix.OdinInspector.TypeFilter("GetRequesterTypes")]
        public UI.TargetRequester.TargetRequesterBase Requester;
        public ActionTargetPickerBase targetPicker;

        public virtual bool CheckCosts(MobRenderer mobRenderer, RuntimeAction ract)
        {
            // TODO
            return true;
            //mob.data.availableActions.Contains(this) &&
            //!mob.data.IsInGCD(this.GCDgroup) &&
            //CooldownRemain <= 0;
        }

        public void DoCosts(MobRenderer mobRenderer)
        {
            // TODO
        }

        // Validation related (ActionValidatorSO) + Target Requester
        //[InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        //public ActionValidatorSO validator;

        public virtual bool Equipable(MobData mobdata) { return true; }
        public virtual bool Check(MobData mob) { return true; }

        public virtual bool CheckWithTargets(MobData mob, Spells.SpellTarget target)
        {
            // TODO
            if (Requester == null)
            {
                return true;
            }
            return Requester.CheckTargets(mob, target);
        }

        public IEnumerable<System.Type> GetRequesterTypes()
        {
            var q = typeof(UI.TargetRequester.TargetRequesterBase).Assembly.GetTypes()
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsGenericTypeDefinition)
            .Where(x => typeof(UI.TargetRequester.TargetRequesterBase).IsAssignableFrom(x));

            return q;
        }

        // Functions etc.
        [Title("Behavioural Parameters")] [SerializeField] private None _;
        // [Sirenix.Serialization.OdinSerialize]
        // [EventSlot]
        // public LuaFunc<(GeneralCombatData, MobRenderer, Spells.SpellTarget), IEnumerator> onPerform = new();

        /// <summary>
        /// The coroutine that handles the action when performed.
        /// It should be override by the derived class if desired.
        ///
        /// Use `Globals.cc` to access the CoroutineContext, for e.g., animation settings & random number generators.
        /// </summary>
        /// <param name="ract">The RuntimeAction instance wrapped from this ActionDataSO that will be performed.</param>
        /// <param name="mob">MobData performing the action.</param>
        /// <param name="target">Target (List of Vector3Ints) of this action.</param>
        /// <returns>No return values.</returns>
        public virtual IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            yield return -1;
        }

        [Obsolete("Use RuntimeAction.Do instead.")]
        public virtual IEnumerator OnPerform(GeneralCombatData combatData, MobRenderer mobRenderer, Spells.SpellTarget target)
        {
            yield break;
            // yield return new JumpIn(onPerform.Eval((combatData, mobRenderer, target)));
        }

        public virtual RuntimeAction LeveledWrap(MobData source, int level)
        {
            return new RuntimeAction(source, this, level);
        }

        //public override bool Equals(object other)
        //{
        //    return Guid == ((ActionDataSO)other).Guid;
        //}

        //public override int GetHashCode()
        //{
        //    return Guid.GetHashCode();
        //}
    }

    [System.Serializable]
    public struct ActionSOEntry
    {
        public ActionDataSO data;
        public int level;
    }
    
    [System.Serializable]
    public struct ActionSOEntry<T> where T : ActionDataSO
    {
        public T data;
        public int level;
        
        public ActionSOEntry ToBase()
        {
            return new ActionSOEntry()
            {
                data = data,
                level = level
            };
        }
    }

    [LuaCallCSharp]
    public class RuntimeAction : MobListener
    {
        public int Level => level;
        public int MaxLevel => data.maxLevel;

        public new ActionDataSO data;
        public virtual Consts.ActionFlags flags => data.flags;

        public virtual Dictionary<
            Cost.Type,
            LuaBoundedGetter<(MobData, Spells.SpellTarget), MobData, double>> costs => data.costs;

        public override MobListenerSO.ListenerType type => MobListenerSO.ListenerType.RuntimeAction;

        public List<(Cost, Cost)> costBounds = new();
        public int cooldownRemain;

        Spells.SpellTarget catchedTarget;

        public dNumber power, auxPower;
        public dNumber hit, crit;
        public GridShape shape;

        GeneralCombatData envData = new();

        [CSharpCallLua]
        public delegate IEnumerator ActionOnPerform(MobRenderer mobRenderer, Spells.SpellTarget target);

        [CSharpCallLua]
        public delegate void Test(MobRenderer mobRenderer);

        string paddedLuaExpr;

        public RuntimeAction(MobData source, ActionDataSO data, int level) : base(source, null)
        {
            this.data = data;
            this.level = level;
        }

        public void SetData(ActionDataSO data)
        {
            this.data = data;
        }

        /* Action perform routine:
         * ActivateInUI
         * > RecalcActionStats() -> Cost bounds
         * > Check if usable by mob (mob can comsume minimum cost)
         * > Pickable in UI; Picked in UI
         * > Target requester (Only legal positions pickable)
         * > Picked target
         * ~ Stat recalc?
         * > mob.ActionPrecheck (OnActionChosen)
         * > DoCost, etc...
         * > Activate()
         * 
         * Activate(src, target, ...)
         * > CheckTarget()
         * > Do()
         */
        // TODO: Move Get delegate to Ctor
        public virtual IEnumerator Do(MobData mob, Spells.SpellTarget target/*, bool cd = true*//*, bool host = false*/)
        {
            //            string postfix = data.Id;
            //            paddedLuaExpr = @$"function routine_{postfix}(mob, target)

            //{data.onPerform.LuaExpr}

            //end

            //function getCsRoutine_{postfix}(mob, target)
            //    return util.cs_generator(routine_{postfix}, mob, target)
            //end";

            //            Debug.Log(paddedLuaExpr);
            //            Globals.xLuaInstance.Instance.luaEnv.DoString(paddedLuaExpr, "Action onPerform chunk");

            //            var testIE = Globals.xLuaInstance.Instance.luaEnv.Global.Get<ActionOnPerform>($"getCsRoutine_{postfix}");
            //            Debug.Log(testIE);

            // Globals.ccNewContext(new SerialCoroutineContext() { animation = true, rng = Globals.cc.rng });
            yield return new JumpIn(data.OnPerform(this, mob, target));

            // Wait a bit for animation
            // yield return new WaitForSeconds(.5f);
        }

        public IEnumerator Activate(MobData mob, Spells.SpellTarget target)
        {
            if(data.CheckWithTargets(mob, target))
            {
                yield return new JumpIn(Do(mob, target));
            }
        }

        public string GetTooltip(MobData mob)
        {
            string costString = "";
            foreach (var costBound in costBounds)
            {
                var lb = costBound.Item1;
                var ub = costBound.Item2;

                if(lb.type != ub.type) { Debug.LogError("Cost bound pair type mismatch, ignored."); continue; }

                if(lb.value == ub.value)
                {
                    costString += $"{lb.type.ToString()} {(int)lb.value.Value}\n";
                }
                else
                {
                    costString += $"{lb.type.ToString()} {(int)lb.value.Value} - {(int)ub.value.Value}\n";
                }
            }

            return $"{costString}" +
                //$"------------\n" +
                $"{data.Description}\n" +
                $"Power: {Mathf.CeilToInt(data.power.Eval(level, mob))}\n" +
                $"TODO - rAct.GetTooltip().";
        }

        public IEnumerator RequestInUI(MobData mob)
        {
            Debug.LogWarning("Refactor UI to Coroutine based as well as Requesters !!!!!");

            if(cooldownRemain > 0)
            {
                Globals.debugMessage.AddMessage($"{mob.nickname} 的 {data.name} 还没有准备好！");
                yield break;
            }

            if(data.Check(mob))
            {
                catchedTarget = null;
                bool canceled = false;

                // Assume max 1 request at once.
                data.Requester.Request(mob, this, (Spells.SpellTarget target) =>
                {
                    catchedTarget = target;
                }, () => { canceled = true; });

                while(catchedTarget == null && canceled == false)
                {
                    yield return null;
                }
                if (canceled) { yield break; }

                List<Cost> cost = costs.Select(pair =>
                new Cost(dNumber.CreateComposite(pair.Value.Eval((mob, catchedTarget))), pair.Key)).ToList();

                yield return new JumpIn(mob.DoAction(this, catchedTarget, cost));

                data.Requester.EndState();
            }
        }

        public void RecalculateStats(MobData mob)
        {
            // 1. Reset stats
            costBounds.Clear();

            foreach (var costEntry in data.costs)
            {
                var bound = costEntry.Value.PrecalculatedBounds(mob);

                Cost lb = new Cost(dNumber.CreateComposite(bound.Item1), costEntry.Key);
                Cost ub = new Cost(dNumber.CreateComposite(bound.Item2), costEntry.Key);

                // Trigger events and re-calc.
                costBounds.Add((
                    mob.GetModifiedCost(lb, this),
                    mob.GetModifiedCost(ub, this)
                ));
            }

            // 2. Compute power etc.
            power = dNumber.CreateComposite(data.power.Eval(level, mob), "actionBase");
            auxPower = dNumber.CreateComposite(data.auxPower.Eval(level, mob), "actionBase");
            
            // TODO: assignable in inspector?
            hit = dNumber.CreateComposite(mob.hitAcc, "actionBase");
            crit = dNumber.CreateComposite(mob.crit, "actionBase");
            
            // TODO: FIXME: Assign gridShape here
            shape = null;

            // After this, we will return to Mob.RecalculateStats();
            // Then Mob.OnActionStatCalculation event will be triggered,
            // which may modify our power values etc.
            // After the event, we go to OnRecalculateStatsFinish().
        }

        public void OnRecalculateStatsFinish(MobData mob)
        {
            if (envData == null) { envData = new(); }

            envData.power = power;
            envData.auxPower = auxPower;
            envData.shape = shape;
            envData.ract = this;
        }

        public virtual void SetCoolDown(int cd)
        {
            cooldownRemain = cd;
        }

        public virtual void OnNextTurn(MobData mob)
        {
            cooldownRemain -= 1;
            if (cooldownRemain < 0) { cooldownRemain = 0; }
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnNextTurn += OnNextTurn;
            mob.OnStatCalculationFinish += OnRecalculateStatsFinish;
        }
    }
}
