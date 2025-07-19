using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using miniRAID;
using miniRAID.Buff;
using Sirenix.Serialization;

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

        [OdinSerialize] private HashSet<Vector2Int> oracleGrids;

        private int oraclesPerTurn => (1 + level);

        public OracleCostsRuntimeBuff(MobData source, OracleCosts data) : base(source, data)
        {
            oracleGrids = new();
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            
            mob.OnCheckCost.AddListener(MobOnCheckCost);
            mob.OnApplyCost.AddListener(MobOnApplyCost);
            mob.OnWakeup.AddListener(MobOnWakeup);
            // mob.OnStatCalculation += MobOnStatCalculation;
        }

        protected override void OnRemoveFromMob(MobData mob)
        {
            mob.OnWakeup.RemoveListener(MobOnWakeup);
            mob.OnApplyCost.RemoveListener(MobOnApplyCost);
            mob.OnCheckCost.RemoveListener(MobOnCheckCost);
            // mob.OnStatCalculation -= MobOnStatCalculation;
            base.OnRemoveFromMob(mob);
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

            UpdateRenderer();
            
            yield break;
        }

        public override void ConstructRenderer()
        {
            renderer = new BatchedRenderer();
            UpdateRenderer();
        }

        public override void UpdateRenderer()
        {
            (renderer as BatchedRenderer)?.Destroy();
            foreach (var grid in oracleGrids)
            {
                (renderer as BatchedRenderer)?.renderers?.Add(SimpleSpriteIndicator.Instantiate(
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
                    UpdateRenderer();
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