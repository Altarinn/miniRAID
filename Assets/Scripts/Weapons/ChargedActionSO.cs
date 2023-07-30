using System.Collections;
using System.Collections.Generic;
using miniRAID.Spells;

namespace miniRAID.Weapon
{
    public abstract class ChargedActionSO : ActionDataSO
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

        public virtual IEnumerator OnPerformCharge(RuntimeAction ract, MobData mob, SpellTarget target)
        {
            yield return new JumpIn(mob.SetActive(false));
        }

        public abstract IEnumerator OnPerformChargedAttack(RuntimeAction ract, MobData mob, SpellTarget target);

        public override RuntimeAction LeveledWrap(MobData source, int level)
        {
            return new ChargedAction(source, this, level);
        }
    }

    public class ChargedAction : RuntimeAction
    {
        protected ChargedActionSO chargedActionData => (ChargedActionSO)data;

        public SpellTarget target;

        public override Consts.ActionFlags flags
        {
            get
            {
                if (isCharging)
                {
                    return chargedActionData.chargedFlags;
                }
                else
                {
                    return base.flags;
                }
            }
        }

        public ChargedAction(MobData source, ChargedActionSO data, int level) : base(source, data, level)
        { }
        
        public bool isCharging = false;
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

        public void Charge(SpellTarget target, MobData mob)
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

        public override IEnumerator Do(MobData mob, SpellTarget target)
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