using System.Collections;
using System.Collections.Generic;
using miniRAID.Agents;
using miniRAID.Spells;

namespace miniRAID.Weapon
{
    public abstract class ChargedActionSO<TSpellTarget> : ActionDataSO<TSpellTarget>
        where TSpellTarget : SpellTarget
    {
        public int chargeTime = 1;
        public int APregenDecrease = 2;

        public Consts.ActionFlags chargedFlags;
        // TODO: Modify costs during charged attack

        public override Dictionary<string, object> LazyPrepareTooltipVariables(RuntimeAction ract)
        {
            var vars = base.LazyPrepareTooltipVariables(ract);
            vars.Add("ChargeTime", chargeTime);
            vars.Add("APRegenDecrease", APregenDecrease);

            return vars;
        }

        public virtual IEnumerator OnPerformCharge(ChargedAction<TSpellTarget> ract, MobData mob, TSpellTarget target)
        {
            mob.FindListener<PlayerAutoAttackAgentBase>()?.SkipNextTurn();
            yield return new JumpIn(mob.SetActive(false));
        }

        public abstract IEnumerator OnPerformChargedAttack(ChargedAction<TSpellTarget> ract, MobData mob, TSpellTarget target);

        public override RuntimeAction<TSpellTarget> LeveledWrap(MobData source, int level)
        {
            return new ChargedAction<TSpellTarget>(source, this, level);
        }
    }

    public interface IChargedAction
    {
        public bool IsCharging { get; }
    }

    public class ChargedAction<TSpellTarget> : RuntimeAction<TSpellTarget>, IChargedAction
        where TSpellTarget : SpellTarget
    {
        protected ChargedActionSO<TSpellTarget> chargedActionData => (ChargedActionSO<TSpellTarget>)data;

        public TSpellTarget target;

        public override Consts.ActionFlags Flags
        {
            get
            {
                if (isCharging)
                {
                    return chargedActionData.chargedFlags;
                }
                else
                {
                    return base.Flags;
                }
            }
        }

        public ChargedAction(MobData source, ChargedActionSO<TSpellTarget> data, int level) : base(source, data, level)
        { }
        
        public bool isCharging = false;

        public bool IsCharging
        {
            get { return isCharging; }
        }

        int chargeTimer = 0;
        
        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnWakeup += MobOnWakeup;
            mob.OnStatCalculation += MobOnOnStatCalculation;
        }

        private void MobOnOnStatCalculation(MobData mob)
        {
            if (isCharging)
            {
                mob.apRecovery -= chargedActionData.APregenDecrease;
            }
        }

        public override void OnRemove(MobData mob)
        {
            mob.OnWakeup -= MobOnWakeup;
            mob.OnStatCalculation -= MobOnOnStatCalculation;
            
            base.OnRemove(mob);
        }

        public IEnumerator MobOnWakeup(MobData mob)
        {
            if(this.isCharging)
            {
                this.chargeTimer--;
                if (chargeTimer <= 0)
                {
                    yield return new JumpIn(mob.DoAction(this, this.target, null));
                }
            }

            yield break;
        }

        public void Charge(TSpellTarget target, MobData mob)
        {
            if (isCharging) return;
            isCharging = true;
            this.target = target;
            chargeTimer = chargedActionData.chargeTime;
        }
        
        public bool Uncharge(MobData mob)
        {
            if (isCharging)
            {
                isCharging = false;
                chargeTimer = 0;
                return true;
            }
            return false;
        }

        public override IEnumerator Do(MobData mob, TSpellTarget target)
        {
            if (isCharging == false)
            {
                yield return new JumpIn(chargedActionData.OnPerformCharge(this, mob, target));
                Charge(target, mob);
            }
            else
            {
                yield return new JumpIn(chargedActionData.OnPerformChargedAttack(this, mob, target));
                Uncharge(mob);
            }

            yield break;
        }
    }
}