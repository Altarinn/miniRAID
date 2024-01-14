﻿using System;
using System.Collections;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;
using Cinemachine;
using miniRAID.Backend.Numericals;

namespace miniRAID.Buff
{
    [System.Serializable]
    public struct BuffSOEntry
    {
        public BuffSO data;
        public int level;
    }
    
    [CreateAssetMenu(menuName = "Buffs/GeneralBuff")]
    public partial class BuffSO : StatModifierSO
    {
        [Title("Buff", "MobListener", TitleAlignments.Centered)]

        public bool timed = false;

        [PathBell]
        public float testValue;

        [ShowIf("timed")]
        public bool phaseTimed = false;
        public int timeMax = 1;

        public bool stackable = false;
        public bool stackRefreshesTime = true;
        public int maxStack = 1;
        public bool snapShot = true;

        public Sprite alwaysOnIndicator = null;
        
        // private float hit, crit;
        
        [Title("Flags")] 
        public Consts.BuffFlags flags;

        [Space(15)]
        [Title("Buff Effects")]
        public None BuffEffects;

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
        [PropertyOrder(100)]
        public List<BuffDHOTDef> overtimeEffects = new();

        #region EventSlots

        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // [Title("General")]
        // public LuaGetter<(Buff, MobData), IEnumerator> onNextTurn = new();
        //
        //
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // [Title("Stat calc")]
        // public LuaGetter<(Buff, MobData), None> onBaseStatCalculation = new();
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // public LuaGetter<(Buff, MobData), None> onStatCalculation = new();
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // public LuaGetter<(Buff, MobData), None> onStatCalculationFinish = new();
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // public LuaGetter<(Buff, MobData), None> onActionStatCalculation = new();
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // public LuaGetter<(Buff, MobData, HashSet<RuntimeAction> actions), None> onQueryActions = new();
        //
        //
        //
        // // TODO: Combat (Dmg / Heal)
        //
        //
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // [Title("Action")]
        // public LuaGetter<(Buff, MobData, RuntimeAction, Spells.SpellTarget), IEnumerator> onActionChosen = new();
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // public LuaGetter<(Buff, MobData, RuntimeAction, Spells.SpellTarget), IEnumerator> onActionPrecast = new();
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // public LuaGetter<(Buff, MobData, RuntimeAction, Spells.SpellTarget), IEnumerator> onActionPostcast = new();
        //
        //
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // [Title("Cost")]
        // public LuaGetter<(Buff, Cost, RuntimeAction, MobData), None> onCostQuery = new();
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // public LuaGetter<(Buff, Cost, RuntimeAction, MobData), None> onCostQueryDisplay = new();
        //
        // [TabGroup("Mob Events")]
        // [PropertyOrder(100)]
        // [EventSlot]
        // public LuaGetter<(Buff, Cost, RuntimeAction, MobData), IEnumerator> onCostApply = new();

        #endregion

        public BuffSO()
        {
            base.type = ListenerType.Buff;
        }

        public override MobListener Wrap(MobData parent)
        {
            return new Buff(parent, this);
        }

        public MobListener LeveledWrap(MobData parent, int level)
        {
            var result = Wrap(parent);
            result.level = level;

            return result;
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

    public class Buff : StatModifier
    {
        public int timeRemain, stacks;
        protected BuffSO buffData => (BuffSO)data;
        
        public Consts.BuffFlags flags => buffData.flags;
        
        [ReadOnly]
        public dNumber crit, hit;

        [HideInInspector]
        public MobData source;

        public string detailedName
        {
            get
            {
                string prefix = "";
                if (stacks > 1)
                {
                    prefix += $"{stacks}x ";
                }

                string postfix = "";
                if (buffData.timed)
                {
                    postfix += $" ({timeRemain}T)";
                }

                return $"{prefix}{buffData.name}{postfix}";
            }
        }

        protected delegate void OnRemoved(MobData mob);
        protected event OnRemoved onRemoveFromMob;

        public Buff(MobData source, BuffSO data) : base(source, data)
        {
            this.source = source;
            
            this.power = dNumber.CreateComposite(buffData.power.Eval(level, source), "buffbase");
            this.auxPower = dNumber.CreateComposite(buffData.auxPower.Eval(level, source), "buffbase");
            // this.hit = dNumber.CreateComposite(buffData.hit.Eval(source), "buffbase");
            // this.crit = dNumber.CreateComposite(buffData.crit.Eval(source), "buffBase");
            this.hit = dNumber.CreateComposite(source.hitAcc, "buffbase");
            this.crit = dNumber.CreateComposite(source.crit, "buffbase");
            
            if(buffData.timed)
            {
                this.timeRemain = buffData.timeMax;
            }

            this.stacks = 1;
        }

        protected override void UpdatePower(MobData mob)
        {
            if (!buffData.snapShot)
            {
                base.UpdatePower(source);
            }
            
            // If snapshot has been enabled, don't modify anything and return
            return;
        }

        public virtual bool Stack()
        {
            if(stacks < buffData.maxStack)
            {
                this.stacks++;
                
                // For proper naming in popup-text / logs
                if (buffData.stackRefreshesTime) { Refresh(); }
                
                return true;
            }
            
            if (buffData.stackRefreshesTime) { Refresh(); }
            
            return false;
        }
        
        public virtual bool RemoveStacks(int amount)
        {
            if (stacks > amount)
            {
                stacks -= amount;
                return true;
            }
            else
            {
                Destroy();
                return false;
            }
        }

        public virtual bool Refresh()
        {
            this.timeRemain = buffData.timeMax;
            Globals.combatTracker.Record(new Consts.BuffEvents()
            {
                buff = this,
                eventType = Consts.BuffEventType.Refreshed
            });
            return true;
        }

        // TODO: cache all functions and unbind them in OnRemove.
        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnNextTurn += BuffBase_OnNextTurn;
            mob.OnRecoveryStage += BuffBase_OnRecoveryStage;

            // TODO: Separate me into something else
            if (Globals.cc.animation && buffData.alwaysOnIndicator)
            {
                AddIndicator(new SimpleSpriteIndicator(
                    buffData.alwaysOnIndicator,
                    Globals.backend.GridToWorldPosCentered(mob.Position), 3))
                    .Move(new Vector3(0, 0, 0.0f))
                    .Follow(mob.mobRenderer);
            }

            // Register events
            // Stats
            // if(buffData.onBaseStatCalculation.isNonEmpty())
            // {
            //     MobData.MobArgumentDelegate evt = (m) =>
            //     {
            //         buffData.onBaseStatCalculation?.Eval((this, m));
            //     };
            //     mob.OnBaseStatCalculation += evt;
            //     onRemoveFromMob += (m) => { m.OnBaseStatCalculation -= evt; };
            // }
            //
            // if (buffData.onStatCalculation.isNonEmpty())
            // {
            //     MobData.MobArgumentDelegate evt = (m) =>
            //     {
            //         buffData.onStatCalculation?.Eval((this, m));
            //     };
            //     mob.OnStatCalculation += evt;
            //     onRemoveFromMob += (m) => { m.OnStatCalculation -= evt; };
            // }
            //
            // if (buffData.onActionStatCalculation.isNonEmpty())
            // {
            //     MobData.MobArgumentDelegate evt = (m) =>
            //     {
            //         buffData.onActionStatCalculation?.Eval((this, m));
            //     };
            //     mob.OnActionStatCalculation += evt;
            //     onRemoveFromMob += (m) => { m.OnActionStatCalculation -= evt; }; 
            // }
            //
            // if (buffData.onStatCalculationFinish.isNonEmpty())
            // {
            //     MobData.MobArgumentDelegate evt = (m) =>
            //     {
            //         buffData.onStatCalculationFinish?.Eval((this, m));
            //     };
            //     mob.OnStatCalculationFinish += evt;
            //     onRemoveFromMob += (m) => { m.OnStatCalculationFinish -= evt; };
            // }
            //
            // if (buffData.onQueryActions.isNonEmpty())
            // {
            //     MobData.MobActionQueryDelegate evt = (m, a) =>
            //     {
            //         buffData.onQueryActions?.Eval((this, m, a));
            //     };
            //     mob.OnQueryActions += evt;
            //     onRemoveFromMob += (m) => { m.OnQueryActions -= evt; };
            // }
            //
            //
            // // Action events
            // if (buffData.onActionChosen.isNonEmpty())
            // {
            //     System.Func<MobData, RuntimeAction, Spells.SpellTarget, IEnumerator> evt = (m, a, t) =>
            //          buffData.onActionChosen?.Eval((this, m, a, t));
            //     mob.OnActionChosen += evt;
            //     onRemoveFromMob += (m) => { m.OnActionChosen -= evt; };
            // }
            //
            // if (buffData.onActionPrecast.isNonEmpty())
            // {
            //     System.Func<MobData, RuntimeAction, Spells.SpellTarget, IEnumerator> evt = (m, a, t) =>
            //          buffData.onActionPrecast?.Eval((this, m, a, t));
            //     mob.OnActionPrecast += evt;
            //     onRemoveFromMob += (m) => { m.OnActionPrecast -= evt; };
            // }
            //
            // if (buffData.onActionPostcast.isNonEmpty())
            // {
            //     System.Func<MobData, RuntimeAction, Spells.SpellTarget, IEnumerator> evt = (m, a, t) =>
            //         buffData.onActionPostcast?.Eval((this, m, a, t));
            //     mob.OnActionPostcast += evt;
            //     onRemoveFromMob += (m) => { m.OnActionPostcast -= evt; };
            // }
            //
            //
            // //Cost
            // if (buffData.onCostQuery.isNonEmpty())
            // {
            //     MobData.CostQueryDelegate evt = (c, a, m) =>
            //     { buffData.onCostQuery?.Eval((this, c, a, m)); };
            //     mob.OnCostQuery += evt;
            //     onRemoveFromMob += (m) => { m.OnCostQuery -= evt; };
            // }
            //
            // if (buffData.onCostQueryDisplay.isNonEmpty())
            // {
            //     MobData.CostQueryDelegate evt = (c, a, m) =>
            //     { buffData.onCostQueryDisplay?.Eval((this, c, a, m)); };
            //     mob.OnCostQueryDisplay += evt;
            //     onRemoveFromMob += (m) => { m.OnCostQueryDisplay -= evt; };
            // }
            //
            // if (buffData.onCostApply.isNonEmpty())
            // {
            //     System.Func<Cost, RuntimeAction, MobData, IEnumerator> evt = (c, a, m) =>
            //         buffData.onCostApply?.Eval((this, c, a, m));
            //     mob.OnCostApply += evt;
            //     onRemoveFromMob += (m) => { m.OnCostApply -= evt; };
            // }
            
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
            
            mob.OnNextTurn -= BuffBase_OnNextTurn;
            mob.OnRecoveryStage -= BuffBase_OnRecoveryStage;
            onRemoveFromMob?.Invoke(mob);

            base.OnRemove(mob);
        }

        public virtual void OnNextTurn(MobData mob) { }
        
        public virtual void OnRecoveryStage(MobData mob) { }

        private IEnumerator BuffBase_OnNextTurn(MobData mob)
        {
            // Call buff's "update" here
            OnNextTurn(mob);

            // DHOTs
            foreach (var dhotinfo in buffData.overtimeEffects)
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
                        
                        flags = Consts.DamageHealFlags.Indirect | Consts.DamageHealFlags.OvertimeEffect,

                        popup = true
                }));
            }

            // Custom events
            // yield return new JumpIn(buffData.onNextTurn?.Eval((this, mob)));

            // Handle timer
            if(this.buffData.timed && (!this.buffData.phaseTimed))
            {
                StepTimer();
            }

            yield break;
        }
        
        private IEnumerator BuffBase_OnRecoveryStage(MobData mob)
        {
            // Call buff's "update" here
            OnRecoveryStage(mob);

            // Custom events
            // yield return new JumpIn(buffData.onRecoveryStage?.Eval((this, mob)));

            // Handle timer
            if(this.buffData.timed && this.buffData.phaseTimed)
            {
                StepTimer();
            }

            yield break;
        }

        protected void StepTimer()
        {
            timeRemain -= 1;

            if(timeRemain <= 0)
            {
                Destroy();
            }
        }
    }
}