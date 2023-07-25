using UnityEngine;

namespace miniRAID
{
    public class GeneralManaListenerSO : MobListenerSO
    {
        public override MobListener Wrap(MobData parent)
        {
            return new GeneralManaListener(parent, this);
        }
    }

    public class GeneralManaListener : MobListener
    {
        protected GeneralManaListenerSO manaData => (GeneralManaListenerSO)data;

        public int current;
        public int max = 100;
        
        public GeneralManaListener(MobData parent, MobListenerSO data) : base(parent, data)
        {
            current = max;
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnCheckCost += MobOnCheckCost;
            mob.OnApplyCost += MobOnApplyCost;
            mob.OnStatCalculation += MobOnStatCalculation;
            mob.OnWakeup += MobOnWakeUp;
        }

        public override void OnRemove(MobData mob)
        {
            mob.OnCheckCost -= MobOnCheckCost;
            mob.OnApplyCost -= MobOnApplyCost;
            mob.OnStatCalculation -= MobOnStatCalculation;
            mob.OnWakeup -= MobOnWakeUp;
            
            base.OnRemove(mob);
        }

        public void MobOnStatCalculation(MobData mob)
        {
            float manaPercent = (float)current / max;

            max = mob.MAG * 3 + mob.level * 1;
            current = Mathf.RoundToInt(manaPercent * max);
        }

        private bool MobOnCheckCost(Cost cost, RuntimeAction ract, MobData mob)
        {
            if (cost.type == Cost.Type.Mana)
            {
                return cost.value <= current;
            }

            return false;
        }

        private void MobOnApplyCost(Cost cost, RuntimeAction ract, MobData mob)
        {
            if (cost.type == Cost.Type.Mana)
            {
                current -= cost.value;
            }
        }
        
        private void MobOnWakeUp(MobData mob)
        {
            AddMana(Mathf.FloorToInt(mob.MAG * 0.2f));
        }

        public void AddMana(int value)
        {
            current += value;
            if (current > max) current = max;
        }
    }
}