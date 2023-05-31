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
        public bool stackRefreshesTime = true;
        public int maxStack = 1;

        public LuaGetter<MobData, float> power, auxPower;

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
            public LuaGetter<MobData, float> power;

            [HorizontalGroup]
            [LabelWidth(25)]
            public LuaGetter<MobData, float> crit;
        }

        [TabGroup("DOT - HOTs")]
        public List<BuffDHOTDef> overtimeEffects = new();

        #region EventSlots

        [TabGroup("Mob Events")]
        [EventSlot]
        [Title("General")]
        public LuaGetter<(Buff, MobData), IEnumerator> onNextTurn = new();



        [TabGroup("Mob Events")]
        [EventSlot]
        [Title("Stat calc")]
        public LuaGetter<(Buff, MobData), None> onBaseStatCalculation = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, MobData), None> onStatCalculation = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, MobData), None> onStatCalculationFinish = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, MobData), None> onActionStatCalculation = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, MobData, HashSet<RuntimeAction> actions), None> onQueryActions = new();



        // TODO: Combat (Dmg / Heal)



        [TabGroup("Mob Events")]
        [EventSlot]
        [Title("Action")]
        public LuaGetter<(Buff, MobData, RuntimeAction, Spells.SpellTarget), IEnumerator> onActionChosen = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, MobData, RuntimeAction, Spells.SpellTarget), IEnumerator> onActionPrecast = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, MobData, RuntimeAction, Spells.SpellTarget), IEnumerator> onActionPostcast = new();



        [TabGroup("Mob Events")]
        [EventSlot]
        [Title("Cost")]
        public LuaGetter<(Buff, Cost, RuntimeAction, MobData), None> onCostQuery = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Cost, RuntimeAction, MobData), None> onCostQueryDisplay = new();

        [TabGroup("Mob Events")]
        [EventSlot]
        public LuaGetter<(Buff, Cost, RuntimeAction, MobData), IEnumerator> onCostApply = new();

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
                Buff duplicated = (Buff)(target.FindListener(x => x.data == this));
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

        public override string name
        {
            get
            {
                string prefix = "";
                if (stacks > 1)
                {
                    prefix += $"{stacks}x ";
                }

                string postfix = "";
                if (data.timed)
                {
                    postfix += $" ({timeRemain}T)";
                }

                return $"{prefix}{data.name}{postfix}";
            }
        }

        protected delegate void OnRemoved(MobData mob);
        protected event OnRemoved onRemoveFromMob;

        public Buff(MobData source, BuffSO data) : base(source, data)
        {
            this.data = data;
            this.source = source;
            this.power = dNumber.CreateComposite(data.power.Eval(source), "buffbase");
            this.auxPower = dNumber.CreateComposite(data.auxPower.Eval(source), "buffbase");
            
            if(data.timed)
            {
                this.timeRemain = data.timeMax;
            }

            this.stacks = 1;
        }

        public virtual bool Stack()
        {
            if(stacks < data.maxStack)
            {
                this.stacks++;
                
                // For proper naming in popup-text / logs
                if (data.stackRefreshesTime) { Refresh(); }
                
                return true;
            }
            
            if (data.stackRefreshesTime) { Refresh(); }
            
            return false;
        }

        public virtual bool Refresh()
        {
            this.timeRemain = data.timeMax;
            Globals.combatTracker.Record(new Consts.BuffEvents()
            {
                buff = this,
                eventType = Consts.BuffEventType.Refreshed
            });
            return true;
        }

        public Buff(MobData source, BuffSO data, dNumber power) : base(source, data)
        {
            this.data = data;
            this.source = source;
            this.power = power;
            this.auxPower = dNumber.CreateComposite(data.auxPower.Eval(source), "buffbase");

            if (data.timed)
            {
                this.timeRemain = data.timeMax;
            }

            this.stacks = 1;
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
            
            this.stacks = 1;
        }

        // TODO: cache all functions and unbind them in OnRemove.
        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnNextTurn += BuffBase_OnNextTurn;

            // Register events
            // Stats
            if(data.modifiers.Count > 0 || data.onBaseStatCalculation.isNonEmpty())
            {
                MobData.MobArgumentDelegate evt = (m) =>
                {
                    data.ModifyBaseStats(m, data.modifiers, stacks);
                    data.onBaseStatCalculation?.Eval((this, m));
                };
                mob.OnBaseStatCalculation += evt;
                onRemoveFromMob += (m) => { m.OnBaseStatCalculation -= evt; };
            }

            if (data.modifiers.Count > 0 || data.onStatCalculation.isNonEmpty())
            {
                MobData.MobArgumentDelegate evt = (m) =>
                {
                    data.ModifyMoreStats(m, data.modifiers, stacks);
                    data.onStatCalculation?.Eval((this, m));
                };
                mob.OnStatCalculation += evt;
                onRemoveFromMob += (m) => { m.OnStatCalculation -= evt; };
            }

            if (data.onActionStatCalculation.isNonEmpty())
            {
                MobData.MobArgumentDelegate evt = (m) =>
                {
                    data.onActionStatCalculation?.Eval((this, m));
                };
                mob.OnActionStatCalculation += evt;
                onRemoveFromMob += (m) => { m.OnActionStatCalculation -= evt; }; 
            }

            if (data.onStatCalculationFinish.isNonEmpty())
            {
                MobData.MobArgumentDelegate evt = (m) =>
                {
                    data.onStatCalculationFinish?.Eval((this, m));
                };
                mob.OnStatCalculationFinish += evt;
                onRemoveFromMob += (m) => { m.OnStatCalculationFinish -= evt; };
            }

            if (data.onQueryActions.isNonEmpty())
            {
                MobData.MobActionQueryDelegate evt = (m, a) =>
                {
                    data.onQueryActions?.Eval((this, m, a));
                };
                mob.OnQueryActions += evt;
                onRemoveFromMob += (m) => { m.OnQueryActions -= evt; };
            }


            // Action events
            if (data.onActionChosen.isNonEmpty())
            {
                System.Func<MobData, RuntimeAction, Spells.SpellTarget, IEnumerator> evt = (m, a, t) =>
                     data.onActionChosen?.Eval((this, m, a, t));
                mob.OnActionChosen += evt;
                onRemoveFromMob += (m) => { m.OnActionChosen -= evt; };
            }

            if (data.onActionPrecast.isNonEmpty())
            {
                System.Func<MobData, RuntimeAction, Spells.SpellTarget, IEnumerator> evt = (m, a, t) =>
                     data.onActionPrecast?.Eval((this, m, a, t));
                mob.OnActionPrecast += evt;
                onRemoveFromMob += (m) => { m.OnActionPrecast -= evt; };
            }

            if (data.onActionPostcast.isNonEmpty())
            {
                System.Func<MobData, RuntimeAction, Spells.SpellTarget, IEnumerator> evt = (m, a, t) =>
                    data.onActionPostcast?.Eval((this, m, a, t));
                mob.OnActionPostcast += evt;
                onRemoveFromMob += (m) => { m.OnActionPostcast -= evt; };
            }


            //Cost
            if (data.onCostQuery.isNonEmpty())
            {
                MobData.CostQueryDelegate evt = (c, a, m) =>
                { data.onCostQuery?.Eval((this, c, a, m)); };
                mob.OnCostQuery += evt;
                onRemoveFromMob += (m) => { m.OnCostQuery -= evt; };
            }

            if (data.onCostQueryDisplay.isNonEmpty())
            {
                MobData.CostQueryDelegate evt = (c, a, m) =>
                { data.onCostQueryDisplay?.Eval((this, c, a, m)); };
                mob.OnCostQueryDisplay += evt;
                onRemoveFromMob += (m) => { m.OnCostQueryDisplay -= evt; };
            }

            if (data.onCostApply.isNonEmpty())
            {
                System.Func<Cost, RuntimeAction, MobData, IEnumerator> evt = (c, a, m) =>
                    data.onCostApply?.Eval((this, c, a, m));
                mob.OnCostApply += evt;
                onRemoveFromMob += (m) => { m.OnCostApply -= evt; };
            }
            
            Globals.combatTracker.Record(new Consts.BuffEvents()
            {
                buff = this,
                eventType = Consts.BuffEventType.Attached
            });
        }

        public override void OnRemove(MobData mob)
        {
            Globals.combatTracker.Record(new Consts.BuffEvents()
            {
                buff = this,
                eventType = Consts.BuffEventType.Removed
            });
            
            base.OnRemove(mob);
            mob.OnNextTurn -= BuffBase_OnNextTurn;
            onRemoveFromMob?.Invoke(mob);
        }

        public virtual void OnNextTurn(MobData mob) { }

        private IEnumerator BuffBase_OnNextTurn(MobData mob)
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
                        source = source,
                        sourceBuff = this,

                        //sourceSpell = Spells.Spell.Dummy(name),
                        //sourceSpellEnt = null,

                        value = dhotinfo.power.Eval(mob) * power * stacks,
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