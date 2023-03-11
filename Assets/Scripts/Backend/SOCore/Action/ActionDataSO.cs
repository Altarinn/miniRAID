using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

using Sirenix.OdinInspector;
using System.Linq;

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

    [LuaCallCSharp]
    [CreateAssetMenu(fileName = "ActionData.asset", menuName = "ActionDataSO", order = 0)]
    public class ActionDataSO : CustomIconScriptableObject
    {
        [Title("Basic info")]
        public string ActionName;

        [TextArea(1, 25)]
        public string Description;

        // TODO: Boolean arrays
        // public List<string> Tags;
        public LuaGetter<Mob, float> power, auxPower;

        // Cost related
        [Title("Costs", horizontalLine: true, bold: true)]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine)]
        public Dictionary<
            Cost.Type,
            LuaBoundedGetter<(Mob, Spells.SpellTarget), Mob, double>> costs = new();
        //public LuaGetter<(Mob, Spells.SpellTarget), GCDGroup> gcdGroup = GCDGroup.Common;

        public LuaGetter<Mob, bool> isActivelyUsed = true;

        [Title("Requester & Validation", horizontalLine: true, bold: true)]
        [TypeFilter("GetRequesterTypes")]
        public UI.TargetRequester.TargetRequesterBase Requester;

        public virtual bool CheckCosts(Mob mob, RuntimeAction ract)
        {
            // TODO
            return true;
            //mob.data.availableActions.Contains(this) &&
            //!mob.data.IsInGCD(this.GCDgroup) &&
            //CooldownRemain <= 0;
        }

        public void DoCosts(Mob mob)
        {
            // TODO
        }

        // Validation related (ActionValidatorSO) + Target Requester
        //[InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        //public ActionValidatorSO validator;

        public virtual bool Equipable(MobData mobdata) { return true; }
        public virtual bool Check(Mob mob) { return true; }

        public virtual bool CheckWithTargets(Mob mob, Spells.SpellTarget target)
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
        [Title("Behaviour")]
        [Sirenix.Serialization.OdinSerialize]
        [EventSlot]
        public LuaFunc<(GeneralCombatData, Mob, Spells.SpellTarget), IEnumerator> onPerform = new();

        public virtual IEnumerator OnPerform(GeneralCombatData combatData, Mob mob, Spells.SpellTarget target)
        {
            yield return new JumpIn(onPerform.Eval((combatData, mob, target)));
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

    [LuaCallCSharp]
    public class RuntimeAction : MobListener
    {
        public new ActionDataSO data;

        public override MobListenerSO.ListenerType type => MobListenerSO.ListenerType.RuntimeAction;

        public List<(Cost, Cost)> costBounds = new();
        public int cooldownRemain;

        Spells.SpellTarget catchedTarget;

        public dNumber power, auxPower;
        public GridShape shape;

        GeneralCombatData envData = new();

        [CSharpCallLua]
        public delegate IEnumerator ActionOnPerform(Mob mob, Spells.SpellTarget target);

        [CSharpCallLua]
        public delegate void Test(Mob mob);

        string paddedLuaExpr;

        public RuntimeAction(MobData source, ActionDataSO data) : base(source, null)
        {
            this.data = data;
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
        public IEnumerator Do(Mob mob, Spells.SpellTarget target/*, bool cd = true*//*, bool host = false*/)
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

            yield return new JumpIn(data.OnPerform(envData, mob, target));

            // Wait a bit for animation
            // yield return new WaitForSeconds(.5f);
        }

        public IEnumerator Activate(Mob mob, Spells.SpellTarget target)
        {
            if(data.CheckWithTargets(mob, target))
            {
                yield return new JumpIn(Do(mob, target));
            }
        }

        public string GetTooltip(Mob mob)
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
                $"Power: {data.power.Eval(mob)}\n" +
                $"TODO - rAct.GetTooltip().";
        }

        public IEnumerator RequestInUI(Mob mob)
        {
            Debug.LogWarning("Refactor UI to Coroutine based as well as Requesters !!!!!");

            if(cooldownRemain > 0)
            {
                Globals.debugMessage.Instance.Message($"{mob.data.nickname} 的 {data.name} 还没有准备好！");
                yield break;
            }

            if(data.Check(mob))
            {
                catchedTarget = null;
                bool canceled = false;

                // Assume max 1 request at once.
                data.Requester.Request(mob, (Spells.SpellTarget target) =>
                {
                    catchedTarget = target;
                }, () => { canceled = true; });

                while(catchedTarget == null && canceled == false)
                {
                    yield return null;
                }
                if (canceled) { yield break; }

                List<Cost> cost = data.costs.Select(pair =>
                new Cost(dNumber.CreateComposite(pair.Value.Eval((mob, catchedTarget))), pair.Key)).ToList();

                yield return new JumpIn(mob.DoAction(this, catchedTarget, cost));

                data.Requester.EndState();
            }
        }

        public void RecalculateStats(Mob mob)
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
            power = dNumber.CreateComposite(data.power.Eval(mob), "actionBase");
            auxPower = dNumber.CreateComposite(data.auxPower.Eval(mob), "actionBase");
            // TODO: FIXME: Assign gridShape here
            shape = null;

            // After this, we will return to Mob.RecalculateStats();
            // Then Mob.OnActionStatCalculation event will be triggered,
            // which may modify our power values etc.
            // After the event, we go to OnRecalculateStatsFinish().
        }

        public void OnRecalculateStatsFinish(Mob mob)
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

        public virtual void OnNextTurn(Mob mob)
        {
            cooldownRemain -= 1;
            if (cooldownRemain < 0) { cooldownRemain = 0; }
        }

        public override void OnAttach(Mob mob)
        {
            base.OnAttach(mob);

            mob.OnNextTurn += OnNextTurn;
            mob.OnStatCalculationFinish += OnRecalculateStatsFinish;
        }
    }
}
