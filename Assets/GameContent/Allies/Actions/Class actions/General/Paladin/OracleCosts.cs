using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.Buff;

namespace GameContent.Buffs.Test
{
    public class OracleCosts : PassiveSkillSO
    {
        public Sprite indicator;
        public BuffSO oracleBuffSO;

        public override MobListener Wrap(MobData parent)
        {
            return new OracleCostsRuntimeBuff(parent, this);
        }
    }

    public class OracleCostsRuntimeBuff : Buff
    {
        protected OracleCosts oracleData => (OracleCosts)data;

        private HashSet<Vector2Int> oracleGrids;

        private int oraclesPerTurn => (1 + level);

        public OracleCostsRuntimeBuff(MobData source, OracleCosts data) : base(source, data)
        {
            oracleGrids = new();
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            
            mob.OnCheckCost += MobOnCheckCost;
            mob.OnApplyCost += MobOnApplyCost;
            mob.OnWakeup += MobOnWakeup;
            // mob.OnStatCalculation += MobOnStatCalculation;

            onRemoveFromMob += m =>
            {
                mob.OnWakeup -= MobOnWakeup;
                mob.OnApplyCost -= MobOnApplyCost;
                mob.OnCheckCost -= MobOnCheckCost;
                // mob.OnStatCalculation -= MobOnStatCalculation;
            };
        }
        
        private IEnumerator MobOnWakeup(MobData mob)
        {
            // Generate oracle grids
            oracleGrids.Clear();
            for (int i = 0; i < oraclesPerTurn; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    Vector2Int gridPos = new Vector2Int(
                        mob.Position.x + Globals.cc.rng.NextInt(-5, 5),
                        mob.Position.z + Globals.cc.rng.NextInt(-5, 5));

                    if (!oracleGrids.Contains(gridPos) &&
                        Globals.backend.InMap(new Vector3Int(gridPos.x, 0, gridPos.y)))
                    {
                        oracleGrids.Add(gridPos);
                        break;
                    }
                }
            }

            RefreshIndicators();
            
            yield break;
        }

        void RefreshIndicators()
        {
            RemoveAllIndicators();
            foreach (var grid in oracleGrids)
            {
                AddIndicator(new SimpleSpriteIndicator(
                        oracleData.indicator,
                        Globals.backend.GridToWorldPosCentered(new Vector3Int(grid.x, 0, grid.y))));
            }
        }

        private bool MobOnCheckCost(Cost cost, RuntimeAction ract, MobData mob)
        {
            switch (cost.type)
            {
                case Cost.Type.OracleGrid:
                    return oracleGrids.Contains(new Vector2Int(mob.Position.x, mob.Position.z));
                case Cost.Type.OracleBuff:
                {
                    Buff buff = mob.FindListener(oracleData.oracleBuffSO) as Buff;
                    if (buff == null)
                    {
                        return false;
                    }
                
                    return buff.stacks >= cost.value;
                }
                default:
                    return false;
            }
        }

        private IEnumerator MobOnApplyCost(Cost cost, RuntimeAction ract, MobData mob)
        {
            switch (cost.type)
            {
                case Cost.Type.OracleGrid:
                    oracleGrids.Remove(new Vector2Int(mob.Position.x, mob.Position.z));
                    RefreshIndicators();
                    break;
                case Cost.Type.OracleBuff:
                {
                    if (mob.FindListener(oracleData.oracleBuffSO) is Buff buff)
                    {
                        buff.RemoveStacks(cost.value);
                    }

                    break;
                }
            }
            yield break;
        }
    }
}