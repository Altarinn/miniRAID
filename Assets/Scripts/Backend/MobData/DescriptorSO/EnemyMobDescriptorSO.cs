using Sirenix.OdinInspector;
using UnityEngine;

namespace miniRAID
{
    [CreateAssetMenu(fileName = "EnemyDescriptor", menuName = "Enemy descriptor")]
    public class EnemyMobDescriptorSO : BaseMobDescriptorSO
    {
        public struct EnemyStatBase
        {
            public int MaxHP;
            public float ExHitAcc;
            public float ExCrit;
            public float ExDodge;
            public float ExAntiCrit;
        }

        protected static float AverageDefense(int level)
        {
            return Consts.GetIdenticalDefense(level);
        }

        protected static float AverageHealth(int level)
        {
            return Consts.GetHealth(level, Consts.BaseStatsFromLevel(level, Consts.baseStatAverageGrowth));
        }

        protected static float BaseHitAccPerLevel = Consts.HitRangePerLevel * 0.6f;
        protected static float BaseDodgePerLevel = 0.0f;
        protected static float BaseCritPerLevel = 0.0f;
        protected static float BaseAntiCritPerLevel = 0.0f;

        protected static float BaseAttackPowerToTargetHPRatio = 1.0f / 4.0f;
        
        [PropertyOrder(-1)]
        public EnemyStatBase baseEnemyStats;

        public override void RecalculateMobBaseStats(MobData mob)
        {
            mob.baseStats.VIT = dNumber.CreateComposite(0, "mobbase");
            mob.baseStats.STR = dNumber.CreateComposite(0, "mobbase");
            mob.baseStats.MAG = dNumber.CreateComposite(0, "mobbase");
            mob.baseStats.INT = dNumber.CreateComposite(0, "mobbase");
            mob.baseStats.DEX = dNumber.CreateComposite(0, "mobbase");
            mob.baseStats.TEC = dNumber.CreateComposite(0, "mobbase");
        }

        public override void RecalculateMobBattleStats(MobData mob)
        {
            // TODO: Make those stats reflect buffs applied to base stats
            // TODO: Fill Battle stats with Basic calculations
            mob.battleStats = new Consts.BattleStats();
            
            mob.maxHealth = dNumber.CreateComposite(baseEnemyStats.MaxHP, "mobbase");
            mob.moveRange = dNumber.CreateComposite(moveRange, "mobbase");

            mob.defense = dNumber.CreateComposite(0, "mobbase");
            mob.spDefense = dNumber.CreateComposite(0, "mobbase");

            /*
             * Set power of enemy's attack to 100 => Will OHKO average character.
             * In UI, this will show as $"dealing {attackPower * action.powerRatio / BaseDefense(level)} damage".
             * Correspondingly, the defense rate of a character will be shown as "${1 - BaseDef(Lv) / mob.def}".
             */
            mob.attackPower =
                dNumber.CreateComposite(
                    AverageDefense(level) * AverageHealth(level) / level * BaseAttackPowerToTargetHPRatio, "mobbase");
            mob.spellPower =
                dNumber.CreateComposite(
                    AverageDefense(level) * AverageHealth(level) / level * BaseAttackPowerToTargetHPRatio, "mobbase");
            
            // DEBUG ONLY
            mob.healPower = dNumber.CreateComposite(1, "mobbase");
            // DEBUG ONLY ENDS

            mob.hitAcc = dNumber.CreateComposite((baseEnemyStats.ExHitAcc + BaseHitAccPerLevel) * level, "mobbase");
            mob.crit = dNumber.CreateComposite((baseEnemyStats.ExCrit + BaseCritPerLevel) * level, "mobbase");
            mob.dodge = dNumber.CreateComposite((baseEnemyStats.ExDodge + BaseDodgePerLevel) * level, "mobbase");
            mob.antiCrit = dNumber.CreateComposite((baseEnemyStats.ExAntiCrit + BaseAntiCritPerLevel) * level, "mobbase");

            mob.aggroMul = dNumber.CreateComposite(1.0, "mobbase");
        }
    }
}