using miniRAID;
using miniRAID.Buff;

namespace GameContent.Buffs.Test
{
    public class WindBuffSO : BuffSO
    {
        public override MobListener Wrap(MobData parent)
        {
            return new WindBuff(parent, this);
        }
    }

    public class WindBuff : Buff
    {
        public WindBuff(MobData source, BuffSO data) : base(source, data)
        {}

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            
            mob.OnCostQuery += MobOnCostQuery;
            onRemoveFromMob += m =>
            {
                m.OnCostQuery -= MobOnCostQuery;
            };
        }

        private void MobOnCostQuery(Cost cost, RuntimeAction ract, MobData mob)
        {
            if (cost.type == Cost.Type.AP)
            {
                if (cost.value > 1)
                {
                    cost.Substract((dNumber)1);
                }
            }
        }
    }
}