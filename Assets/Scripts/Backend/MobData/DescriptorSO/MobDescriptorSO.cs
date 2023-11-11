using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace miniRAID
{
    [CreateAssetMenu(fileName = "MobDescriptor", menuName = "Mob descriptor")]
    public class MobDescriptorSO : BaseMobDescriptorSO
    {
        [FormerlySerializedAs("baseStatsFundamental")] [PropertyOrder(-1)]
        public Consts.BaseStatsInt baseStats;

        public float healPriority;

        public override void RecalculateMobBaseStats(MobData mob)
        {
            mob.baseStats.VIT = dNumber.CreateComposite(baseStats.VIT, "mobbase");
            mob.baseStats.STR = dNumber.CreateComposite(baseStats.STR, "mobbase");
            mob.baseStats.MAG = dNumber.CreateComposite(baseStats.MAG, "mobbase");
            mob.baseStats.INT = dNumber.CreateComposite(baseStats.INT, "mobbase");
            mob.baseStats.AGI = dNumber.CreateComposite(baseStats.DEX, "mobbase");
            mob.baseStats.TEC = dNumber.CreateComposite(baseStats.TEC, "mobbase");
        }

        public override void RecalculateMobBattleStats(MobData mob)
        {
            // TODO: Fill Battle stats with Basic calculations
            mob.battleStats = new Consts.BattleStats();
            
            mob.maxHealth = dNumber.CreateComposite(Consts.GetHealth(mob.level, mob.VIT), "mobbase");
            mob.moveRange = dNumber.CreateComposite(moveRange, "mobbase");

            mob.defense = dNumber.CreateComposite(0, "mobbase");
            mob.spDefense = dNumber.CreateComposite(0, "mobbase");

            mob.attackPower = dNumber.CreateComposite(mob.STR, "mobbase");
            mob.spellPower = dNumber.CreateComposite(mob.INT, "mobbase");
            
            // DEBUG ONLY
            mob.healPower = dNumber.CreateComposite(mob.INT, "mobbase");
            mob.buffPower = dNumber.CreateComposite(mob.TEC, "mobbase");
            // DEBUG ONLY ENDS

            mob.hitAcc = dNumber.CreateComposite(mob.TEC, "mobbase");
            mob.crit = dNumber.CreateComposite(mob.TEC, "mobbase");
            mob.dodge = dNumber.CreateComposite(mob.AGI, "mobbase");
            mob.antiCrit = dNumber.CreateComposite(mob.AGI, "mobbase");

            mob.aggroMul = dNumber.CreateComposite(1.0, "mobbase");

            mob.healPriority = healPriority;

            mob.apRecovery = Consts.baseAPRegenTurn + mob.AGI * Consts.APRegenPerDEX;
        }
    }
}