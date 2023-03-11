using System;
using System.Collections;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace miniRAID.Buff
{
    [CreateAssetMenu(menuName = "Buffs/GeneralBuff")]
    public partial class BuffSO : MobListenerSO
    {
        [Title("Buff", "MobListener", TitleAlignments.Centered)]

        public bool timed = false;
        public int timeMax = 1;

        public bool stackable = false;
        public int maxStack = 1;

        public LuaGetter<Mob, float> power, auxPower;

        [Space(15)]
        [Title("Buff Effects")]
        public None BuffEffects;

        [TabGroup("Stat Mods")]
        public Dictionary<StatModTarget, dNumberModifier> modifiers = new();

        [InlineProperty(LabelWidth = 52)]
        public struct BuffDHOTDef
        {
            [HorizontalGroup]
            [LabelWidth(32)]
            public Consts.Elements type;

            [HorizontalGroup]
            [LabelText("Power%")]
            public LuaGetter<Mob, float> power;

            [HorizontalGroup]
            [LabelWidth(25)]
            public LuaGetter<Mob, float> crit;
        }

        [TabGroup("DOT - HOTs")]
        public List<BuffDHOTDef> overtimeEffects = new();

        #region EventSlots

        [TabGroup("Mob Events")]
        [EventSlot]
        [Title("General")]
        public LuaGetter<(Buff, Mob), IEnumerator> onNextTurn = new();



        [TabGroup("Mob Events")]
        [EventSlot]
        [Title("Stat calc")]
        public LuaGetter<(Buff, Mob), None> onBaseStatCalculation = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Mob), None> onStatCalculation = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Mob), None> onStatCalculationFinish = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Mob), None> onActionStatCalculation = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Mob, HashSet<RuntimeAction> actions), None> onQueryActions = new();



        // TODO: Combat (Dmg / Heal)



        [TabGroup("Mob Events")]
        [EventSlot]
        [Title("Action")]
        public LuaGetter<(Buff, Mob, RuntimeAction, Spells.SpellTarget), IEnumerator> onActionChosen = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Mob, RuntimeAction, Spells.SpellTarget), IEnumerator> onActionPrecast = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Mob, RuntimeAction, Spells.SpellTarget), IEnumerator> onActionPostcast = new();



        [TabGroup("Mob Events")]
        [EventSlot]
        [Title("Cost")]
        public LuaGetter<(Buff, Cost, RuntimeAction, Mob), None> onCostQuery = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Cost, RuntimeAction, Mob), None> onCostQueryDisplay = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Cost, RuntimeAction, Mob), IEnumerator> onCostApply = new();

        #endregion

        public BuffSO()
        {
            base.type = ListenerType.Buff;
        }

        public override MobListener Wrap(MobData parent)
        {
            return new Buff(parent, this);
        }

        public override bool TryAdd(MobData target)
        {
            if(base.TryAdd(target))
            {
                // Find myself in target mob
                Buff duplicated = (Buff)(target.listeners.Find(x => x.data == this));
                if(duplicated != null)
                {
                    if (stackable)
                    {
                        if (duplicated.Stack()) { /*Success!*/ }
                    }
                    else
                    {
                        if (duplicated.Refresh()) { /*Success!*/ }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class Buff : MobListener
    {
        public int timeRemain, stacks;
        public new BuffSO data;

        public dNumber power, auxPower, crit, hit;

        [HideInInspector]
        public MobData source;

        delegate void OnRemoved(Mob mob);
        event OnRemoved onRemoveFromMob;

        public Buff(MobData source, BuffSO data) : base(source, data)
        {
            this.data = data;
            this.source = source;
            this.power = dNumber.CreateComposite(data.power.Eval(source.parentMob), "buffbase");
            this.auxPower = dNumber.CreateComposite(data.auxPower.Eval(source.parentMob), "buffbase");
            
            if(data.timed)
            {
                this.timeRemain = data.timeMax;
            }
        }

        public virtual bool Stack()
        {
            if(stacks < data.maxStack)
            {
                this.stacks++;
                return true;
            }
            return false;
        }

        public virtual bool Refresh()
        {
            this.timeRemain = data.timeMax;
            return true;
        }

        public Buff(MobData source, BuffSO data, dNumber power) : base(source, data)
        {
            this.data = data;
            this.source = source;
            this.power = power;
            this.auxPower = dNumber.CreateComposite(data.auxPower.Eval(source.parentMob), "buffbase");

            if (data.timed)
            {
                this.timeRemain = data.timeMax;
            }
        }

        public Buff(MobData source, BuffSO data, dNumber power, dNumber auxPower) : base(source, data)
        {
            this.data = data;
            this.source = source;
            this.power = power;
            this.auxPower = auxPower;

            if (data.timed)
            {
                this.timeRemain = data.timeMax;
            }
        }

        // TODO: cache all functions and unbind them in OnRemove.
        public override void OnAttach(Mob mob)
        {
            base.OnAttach(mob);

            mob.OnNextTurn += BuffBase_OnNextTurn;

            // Register events
            // Stats
            if(data.modifiers.Count > 0 || data.onBaseStatCalculation.isNonEmpty())
            {
                Mob.MobArgumentDelegate evt = (m) =>
                {
                    data.ModifyBaseStats(m, data.modifiers);
                    data.onBaseStatCalculation?.Eval((this, m));
                };
                mob.OnBaseStatCalculation += evt;
                onRemoveFromMob += (m) => { m.OnBaseStatCalculation -= evt; };
            }

            if (data.modifiers.Count > 0 || data.onStatCalculation.isNonEmpty())
            {
                Mob.MobArgumentDelegate evt = (m) =>
                {
                    data.ModifyMoreStats(m, data.modifiers);
                    data.onStatCalculation?.Eval((this, m));
                };
                mob.OnStatCalculation += evt;
                onRemoveFromMob += (m) => { m.OnStatCalculation -= evt; };
            }

            if (data.onActionStatCalculation.isNonEmpty())
            {
                Mob.MobArgumentDelegate evt = (m) =>
                {
                    data.onActionStatCalculation?.Eval((this, m));
                };
                mob.OnActionStatCalculation += evt;
                onRemoveFromMob += (m) => { m.OnActionStatCalculation -= evt; }; 
            }

            if (data.onStatCalculationFinish.isNonEmpty())
            {
                Mob.MobArgumentDelegate evt = (m) =>
                {
                    data.onStatCalculationFinish?.Eval((this, m));
                };
                mob.OnStatCalculationFinish += evt;
                onRemoveFromMob += (m) => { m.OnStatCalculationFinish -= evt; };
            }

            if (data.onQueryActions.isNonEmpty())
            {
                Mob.MobActionQueryDelegate evt = (m, a) =>
                {
                    data.onQueryActions?.Eval((this, m, a));
                };
                mob.OnQueryActions += evt;
                onRemoveFromMob += (m) => { m.OnQueryActions -= evt; };
            }


            // Action events
            if (data.onActionChosen.isNonEmpty())
            {
                System.Func<Mob, RuntimeAction, Spells.SpellTarget, IEnumerator> evt = (m, a, t) =>
                     data.onActionChosen?.Eval((this, m, a, t));
                mob.OnActionChosen += evt;
                onRemoveFromMob += (m) => { m.OnActionChosen -= evt; };
            }

            if (data.onActionPrecast.isNonEmpty())
            {
                System.Func<Mob, RuntimeAction, Spells.SpellTarget, IEnumerator> evt = (m, a, t) =>
                     data.onActionPrecast?.Eval((this, m, a, t));
                mob.OnActionPrecast += evt;
                onRemoveFromMob += (m) => { m.OnActionPrecast -= evt; };
            }

            if (data.onActionPostcast.isNonEmpty())
            {
                System.Func<Mob, RuntimeAction, Spells.SpellTarget, IEnumerator> evt = (m, a, t) =>
                    data.onActionPostcast?.Eval((this, m, a, t));
                mob.OnActionPostcast += evt;
                onRemoveFromMob += (m) => { m.OnActionPostcast -= evt; };
            }


            //Cost
            if (data.onCostQuery.isNonEmpty())
            {
                Mob.CostQueryDelegate evt = (c, a, m) =>
                { data.onCostQuery?.Eval((this, c, a, m)); };
                mob.OnCostQuery += evt;
                onRemoveFromMob += (m) => { m.OnCostQuery -= evt; };
            }

            if (data.onCostQueryDisplay.isNonEmpty())
            {
                Mob.CostQueryDelegate evt = (c, a, m) =>
                { data.onCostQueryDisplay?.Eval((this, c, a, m)); };
                mob.OnCostQueryDisplay += evt;
                onRemoveFromMob += (m) => { m.OnCostQueryDisplay -= evt; };
            }

            if (data.onCostApply.isNonEmpty())
            {
                System.Func<Cost, RuntimeAction, Mob, IEnumerator> evt = (c, a, m) =>
                    data.onCostApply?.Eval((this, c, a, m));
                mob.OnCostApply += evt;
                onRemoveFromMob += (m) => { m.OnCostApply -= evt; };
            }
        }

        public override void OnRemove(Mob mob)
        {
            base.OnRemove(mob);
            mob.OnNextTurn -= BuffBase_OnNextTurn;
            onRemoveFromMob?.Invoke(mob);
        }

        public virtual void OnNextTurn(Mob mob) { }

        private IEnumerator BuffBase_OnNextTurn(Mob mob)
        {
            // Call buff's "update" here
            OnNextTurn(mob);

            // DHOTs
            foreach (var dhotinfo in data.overtimeEffects)
            {
                yield return new JumpIn(Globals.backend.DealDmgHeal(mob,
                    new Consts.DamageHeal_FrontEndInput()
                    {
                        //source = source.parentMob,
                        source = source.parentMob,
                        sourceBuff = this,

                        //sourceSpell = Spells.Spell.Dummy(name),
                        //sourceSpellEnt = null,

                        value = dhotinfo.power.Eval(mob) * power,
                        type = dhotinfo.type,

                        crit = dhotinfo.crit.Eval(mob),
                        hit = 500.0f,

                        popup = true
                }));
            }

            // Custom events
            yield return new JumpIn(data.onNextTurn?.Eval((this, mob)));

            // Handle timer
            if(this.data.timed)
            {
                timeRemain -= 1;

                if(timeRemain <= 0)
                {
                    Destroy();
                }
            }

            yield break;
        }
    }
}