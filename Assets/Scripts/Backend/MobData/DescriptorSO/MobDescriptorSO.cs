using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.Serialization;

namespace miniRAID
{
    [CreateAssetMenu(fileName = "MobDescriptor", menuName = "Mob descriptor")]
    public class MobDescriptorSO : BaseMobDescriptorSO
    {
        [FormerlySerializedAs("baseStatsFundamental")] [PropertyOrder(-1)]
        public Consts.BaseStatsInt baseStats;

        public override void RecalculateMobBaseStats(MobData mob)
        {
            mob.baseStats.VIT = dNumber.CreateComposite(baseStats.VIT, "mobbase");
            mob.baseStats.STR = dNumber.CreateComposite(baseStats.STR, "mobbase");
            mob.baseStats.MAG = dNumber.CreateComposite(baseStats.MAG, "mobbase");
            mob.baseStats.INT = dNumber.CreateComposite(baseStats.INT, "mobbase");
            mob.baseStats.DEX = dNumber.CreateComposite(baseStats.DEX, "mobbase");
            mob.baseStats.TEC = dNumber.CreateComposite(baseStats.TEC, "mobbase");
        }

        public override void RecalculateMobBattleStats(MobData mob)
        {
            // TODO: Fill Battle stats with Basic calculations
            mob.battleStats = new Consts.BattleStats();
            
            mob.maxHealth = dNumber.CreateComposite(mob.level * 2 + mob.VIT * 3, "mobbase");

            mob.defense = dNumber.CreateComposite(0, "mobbase");
            mob.spDefense = dNumber.CreateComposite(0, "mobbase");

            mob.attackPower = dNumber.CreateComposite(mob.STR, "mobbase");
            mob.spellPower = dNumber.CreateComposite(mob.INT, "mobbase");
            
            // DEBUG ONLY
            mob.healPower = dNumber.CreateComposite(mob.MAG, "mobbase");
            // DEBUG ONLY ENDS

            mob.hitAcc = dNumber.CreateComposite(mob.TEC, "mobbase");
            mob.crit = dNumber.CreateComposite(mob.TEC, "mobbase");
            mob.dodge = dNumber.CreateComposite(mob.DEX, "mobbase");
            mob.antiCrit = dNumber.CreateComposite(mob.DEX, "mobbase");

            mob.aggroMul = dNumber.CreateComposite(1.0, "mobbase");
        }
    }
}