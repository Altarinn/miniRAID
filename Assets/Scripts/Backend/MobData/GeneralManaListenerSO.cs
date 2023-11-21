using System.Collections;
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
            mob.OnRecoveryStage += MobOnRecoveryStage;
        }

        public override void OnRemove(MobData mob)
        {
            mob.OnCheckCost -= MobOnCheckCost;
            mob.OnApplyCost -= MobOnApplyCost;
            mob.OnStatCalculation -= MobOnStatCalculation;
            mob.OnRecoveryStage -= MobOnRecoveryStage;
            
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

        private IEnumerator MobOnApplyCost(Cost cost, RuntimeAction ract, MobData mob)
        {
            if (cost.type == Cost.Type.Mana)
            {
                current -= cost.value;
            }
            yield break;
        }
        
        private IEnumerator MobOnRecoveryStage(MobData mob)
        {
            AddMana(Mathf.FloorToInt(mob.MAG * 0.2f));
            yield break;
        }

        public void AddMana(int value)
        {
            current += value;
            if (current > max) current = max;
        }
        
        /// <summary>
        /// Use mana directly. Note that in most cases, it is better to use <see cref="MobData.ApplyCost"/> instead.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The actual mana used (may different from the provided value)</returns>
        public int UseMana(int value)
        {
            int used = Mathf.Min(current, value);
            
            current -= used;
            if (current < 0) current = 0;

            return used;
        }
    }
}