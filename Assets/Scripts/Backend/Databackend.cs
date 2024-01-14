using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using miniRAID.Spells;
using miniRAID.Buff;
using System;
using System.Linq;
using miniRAID.Backend;
using miniRAID.Backend.Numericals.Impl;
using UnityEngine.Serialization;

namespace miniRAID
{
    [XLua.LuaCallCSharp]
    public class GridData
    {
        public enum TerrainType
        {
            Normal,
            Mountain
        }

        TerrainType type;

        public bool solid = false;
        public MobData mob;

        public Dictionary<GridEffect, GridEffect> effects = new Dictionary<GridEffect, GridEffect>();
    }

    [XLua.LuaCallCSharp]
    public class GridPath
    {
        public List<Vector3Int> path = new List<Vector3Int>();

        public void Step()
        {
            path.RemoveAt(0);
        }
    }

    public static class StructExts
    {
        public static T Clone<T>(this T val) where T : struct => val;
    }

    [XLua.LuaCallCSharp]
    public static class Consts
    {
        public enum AllElements
        {
            Physical = 16,
            Elemental = 17,
            Pure = 32,

            Slash = 0,
            Knock = 1,
            Pierce = 2,

            Fire = 3,
            Ice = 4,
            Water = 5,
            Nature = 6,
            Wind = 7,
            Thunder = 8,
            Light = 9,
            Dark = 10,

            Heal = 11
        }

        public enum Elements
        {
            Slash = 0,
            Knock = 1,
            Pierce = 2,

            Fire = 3,
            Ice = 4,
            Water = 5,
            Nature = 6,
            Wind = 7,
            Thunder = 8,
            Light = 9,
            Dark = 10,

            Heal = 11
        }

        public static Color MissColor = new Color(1.0f, 0.6f, 0.5f);
        public static Color HitColor = new Color(1.0f, 0.7f, 0.6f);
        public static Color CritColor = new Color(1.0f, 0.88f, 1.0f);
        public static Color HealColor = new Color(0.6f, 1.0f, 0.4f);
        public static Color BuffColor = new Color(0.7f, 0.9f, 1.0f);

        public enum RefType
        {
            Constant = 0,
            Variable = 1,
            Function = 2
        }

        [Flags]
        public enum DamageHealFlags
        {
            // 间接效果（否则为直接效果；直接效果通常可以触发各种特效）
            Indirect = 1 << 1,
            
            // 随时间发动的效果
            OvertimeEffect = 1 << 2,
            
            // 反击效果
            Counter = 1 << 3,
            
            // 追击效果
            FollowUp = 1 << 4,
        }

        public static bool IsDirect(DamageHealFlags flags) => (flags & DamageHealFlags.Indirect) == 0;

        [Flags]
        public enum ActionFlags
        {
            // 无法被打断
            CannotBeInterrupted = 1 << 1,
            
            // 武器的普通攻击
            RegularAction = 1 << 2,
            
            // 武器的特殊攻击
            SpecialAction = 1 << 3,
            
            // 团队技能
            TeamAction = 1 << 4,
            
            // 移动（不算作一般行动）
            Movement = 1 << 5,
        }
        
        [Flags]
        // TODO: Buff dispel types?
        public enum BuffFlags
        {
            // 无法被驱散
            CannotBeDispelled = 1 << 1,
        }

        public class DamageHeal_FrontEndInput
        {
            public MobData source;
            public float value;
            public Elements type;

            public float crit; // crit chance
            public float hit; // Hit chance

            public RuntimeAction sourceAction;
            public Buff.Buff sourceBuff;

            public DamageHealFlags flags;

            public bool IsAction => sourceAction != null;
            // public int Id => IsAction ? sourceAction.data.Id : sourceBuff.data.Id;
            public string Name => IsAction ? sourceAction.data.ActionName : sourceBuff.data.name;
            // public string Description => IsAction ? sourceAction.data.Description : "No description";

            public bool popup;
        }

        // public class DamageHeal_FrontEndInput_ByRef
        // {
        //     public DamageHeal_FrontEndInput value;
        //
        //     public DamageHeal_FrontEndInput_ByRef(DamageHeal_FrontEndInput val)
        //     {
        //         value = val;
        //     }
        // }

        public static bool IsHeal(DamageHeal_FrontEndInput input)
        {
            return input.type == Elements.Heal;
        }

        public class DamageHeal_ComputedRates
        {
            public int value;
            public float hit, crit;
        }
        
        // public class DamageHeal_ComputedRates_ByRef
        // {
        //     public DamageHeal_ComputedRates value;
        //
        //     public DamageHeal_ComputedRates_ByRef(DamageHeal_ComputedRates val)
        //     {
        //         value = val;
        //     }
        // }

        public class DamageHeal_Result
        {
            public MobData source;
            public MobData target;

            public int value;
            public int overdeal;
            public Elements type;

            public bool isCrit;
            public bool isAvoid;
            public bool isBlock;

            public RuntimeAction sourceAction;
            public Buff.Buff sourceBuff;
            
            public DamageHealFlags flags;

            public bool IsAction => sourceAction != null;
            public bool NoInfo => sourceAction == null && sourceBuff == null;
            // public int Id => NoInfo ? -1 : (IsAction ? sourceAction.data.Id : sourceBuff.data.Id);
            public string Name => NoInfo ? "<NULL ACTION>" : (IsAction ? sourceAction.data.ActionName : sourceBuff.data.name);
            // public string Description => NoInfo ? "<NULL ACTION>" : (IsAction ? sourceAction.GetTooltip(source) : "No description");

            public bool popup;
        }

        public static float HealAggroMul = 1.3f;
        
        // This is a multiplier that will be applied to aggro per turn (update).
        public static float AggroDecay = 0.75f;

        public enum BuffEventType
        {
            Attached,
            Stacked,
            Refreshed,
            Removed
        }

        public struct BuffEvents
        {
            public Buff.Buff buff;
            public BuffEventType eventType;
        }

        public struct KillEvent
        {
            public Consts.DamageHeal_Result info;
        }
        
        public struct TrackerActionEvent
        {
            public RuntimeAction action;
            public SpellTarget target;
        }

        // ......
        [Serializable]
        public struct AllTypes<T>
        {
            public T Physical;
            public T Elemental;
            public T Pure;

            public T Slash;
            public T Knock;
            public T Pierce;

            public T Fire;
            public T Ice;
            public T Water;
            public T Nature;
            public T Wind;
            public T Thunder;
            public T Light;
            public T Dark;

            public T Heal;

            public T Get(AllElements e)
            {
                switch(e)
                {
                    case AllElements.Physical:
                        return Physical;
                    case AllElements.Elemental:
                        return Elemental;
                    case AllElements.Pure:
                        return Pure;

                    case AllElements.Slash:
                        return Slash;
                    case AllElements.Knock:
                        return Knock;
                    case AllElements.Pierce:
                        return Pierce;

                    case AllElements.Fire:
                        return Fire;
                    case AllElements.Ice:
                        return Ice;
                    case AllElements.Water:
                        return Water;
                    case AllElements.Nature:
                        return Nature;
                    case AllElements.Wind:
                        return Wind;
                    case AllElements.Thunder:
                        return Thunder;
                    case AllElements.Light:
                        return Light;
                    case AllElements.Dark:
                        return Dark;

                    case AllElements.Heal:
                        return Heal;

                    default:
                        return default;
                }
            }

            public T Get(Elements e) => Get((AllElements)e);
        }

        public static AllElements parentType(AllElements e)
        {
            switch(e)
            {
                case AllElements.Slash:
                case AllElements.Knock:
                case AllElements.Pierce:
                    return AllElements.Physical;
                    break;

                case AllElements.Fire:
                case AllElements.Ice:
                case AllElements.Water:
                case AllElements.Nature:
                case AllElements.Wind:
                case AllElements.Thunder:
                case AllElements.Light:
                case AllElements.Dark:
                    return AllElements.Elemental;
                    break;

                case AllElements.Physical:
                case AllElements.Elemental:
                case AllElements.Pure:
                default:
                    return AllElements.Pure;
                    break;
            }
        }

        public static AllElements parentType(Elements e)
            => parentType((AllElements)e);

        [Serializable]
        public struct BaseStatsInt
        {
            public int VIT, STR, MAG, INT, DEX, TEC;
        }
        
        [Serializable]
        public struct BaseStatsGrowth
        {
            public float VIT, STR, MAG, INT, DEX, TEC;
        }

        [Serializable]
        public struct BaseStats
        {
            public dNumber VIT, STR, MAG, INT;
            [FormerlySerializedAs("DEX")] public dNumber AGI;
            public dNumber TEC;
        }

        // TODO: Determine our values

        public static int maxPlayers = 9, basePlayerPerTurn = 4;

        // Extra level added to attacker level during damage calc.
        // For a smoother early-game experience.
        // However, this may not be a good solution and should be 0 for now.
        // Instead, characters will have significant non-zero stats at Lv1.
        public static int additionalAttackerLevels = 0;

        public static float GetIdenticalDefense(int referenceLevel)
        {
            return referenceLevel + additionalAttackerLevels;
        }
        
        // Incremental in the range of +50% hit rate per level.
        // Hit rate = BaseHit + (attacker.Hit - defender.Dodge) / (HitRangePerLevel * defender.Level)
        public static float HitRangePerLevel = 6;
        public static float BaseHit = 0.55f;
        public static float MaxHitAcc = 1000000.0f;
        
        // Same but for critical strikes.
        public static float CritRangePerLevel = 7;
        public static float BaseCrit = -0.1f;

        public static float APRegenPerDEX = 0.03f;
        public static float baseAPRegenTurn = 1.0f;
        public static float baseAPRegenRecoveryStage = 0.0f;

        public static float GetHitRate(float spellHit, float defenderDodge, int defenderLevel)
        {
            return Mathf.Clamp(BaseHit + (spellHit - defenderDodge) / (HitRangePerLevel * defenderLevel), 0, 1);
        }

        public static float GetCriticalRate(float spellCrit, float defenderAntiCrit, int defenderLevel)
        {
            return Mathf.Clamp(BaseCrit + (spellCrit - defenderAntiCrit) / (CritRangePerLevel * defenderLevel), 0, 1);
        }

        public static int GetDamage(float spellPower, int attackerLevel, float defense, int defenderLevel)
        {
            return Mathf.CeilToInt(spellPower * GetIdenticalDefense(attackerLevel) / (defense + Consts.GetIdenticalDefense(defenderLevel)));
        }
        
        public static float GetDefenseRate(float defense, int defenderLevel)
        {
            return GetIdenticalDefense(defenderLevel) / (defense + GetIdenticalDefense(defenderLevel));
        }
        
        [Serializable]
        public struct BattleStats
        {
            public AllTypes<float> exResist;
            public AllTypes<float> exDamage;
        }
        
        // Maximum levels for skills, buffs, etc
        public static int MaxListenerLevels = 5;
        
        // Standard values at iLvl 10
        private static float SVbaseStats = 10;
        private static float SVpowers = 6.0f;
        private static float SVdefense = 0.3f;
        private static float SVhit = 1.0f;
        
        public static float ValueFromItemLevel(int iLvl, StatModifierSO.StatModTarget entryKey, float val)
        {
            float normalizedItemLV = (float)iLvl / 10.0f;

            switch (entryKey)
            {
                // Main stats
                case StatModifierSO.StatModTarget.VIT:
                case StatModifierSO.StatModTarget.STR:
                case StatModifierSO.StatModTarget.MAG:
                case StatModifierSO.StatModTarget.INT:
                case StatModifierSO.StatModTarget.DEX:
                case StatModifierSO.StatModTarget.TEC:
                    return SVbaseStats * normalizedItemLV * val;
                    break;

                // Sub stats
                // TODO: Add cases for sub stats

                // Battle stats
                case StatModifierSO.StatModTarget.AttackPower:
                case StatModifierSO.StatModTarget.SpellPower:
                case StatModifierSO.StatModTarget.HealPower:
                case StatModifierSO.StatModTarget.BuffPower:
                    return SVpowers * normalizedItemLV * val;
                    break;
                
                case StatModifierSO.StatModTarget.Defense:
                case StatModifierSO.StatModTarget.SpDefense:
                    return SVdefense * normalizedItemLV * val;
                    break;
                
                case StatModifierSO.StatModTarget.AggroMul:
                    break;
                case StatModifierSO.StatModTarget.Hit:
                    return SVhit * normalizedItemLV * val;
                    break;
                case StatModifierSO.StatModTarget.Dodge:
                    break;
                case StatModifierSO.StatModTarget.Crit:
                    break;
                case StatModifierSO.StatModTarget.CritRes:
                    break;
                case StatModifierSO.StatModTarget.ExRange:
                    break;
                case StatModifierSO.StatModTarget.APRegen:
                    break;

                default:
                    // Handle any other cases that are not explicitly listed
                    break;
            }

            return 0f;
        }

        public static float baseStatBaseLv1 = 5;
        public static float baseStatAverageGrowth = 1.0f;

        public static float BaseStatsFromLevel(int lvl, float growthRate)
        {
            return lvl * growthRate + baseStatBaseLv1;
        }

        public static float GetHealth(int lvl, float VIT)
        {
            return lvl * 3 + VIT * 6;
        }

        public static float HealerSelfFocusThresholdHPPercentage = 0.4f;
        public static float HealerSelfFocusPriorityBoost = 1.5f;
        
        public static float GetPrioritizedHealthRatio(MobData source, MobData mob)
        {
            float healP = mob.healPriority;
            if (healP == 0) healP = 1;
            
            if (mob == source && mob.health < mob.maxHealth * Consts.HealerSelfFocusThresholdHPPercentage)
            {
                healP *= Consts.HealerSelfFocusPriorityBoost;
            }

            return (
                (float)mob.health / ((float)mob.maxHealth * healP) // % of max HP, while focusing on self more when in danger
                - 0.01f * mob.healPriority); // Focus on high priority targets when at full HP (or rarely, same %)
        }

        public static float GetPrioritizedHealthRatio(MobData mob) => GetPrioritizedHealthRatio(null, mob);

        public enum UnitGroup
        {
            Player = 0, // Originally players, nothing else allowed
            Enemy = 1, // Enemies
            Ally = 2, // Player's summoned creatures, NPCs etc.
                      // May have agents, in that case they will move as their will instead (NPC)
            Others = 3,
        }

        public static int[] UnitGroupToMaskBit = new int[4] { 1, 2, 4, 8 };

        enum UnitGroupMaskBit
        {
            Player = 1,
            Enemy = 2,
            Ally = 4,
            Others = 8,
        }

        public static int[] Enemies = new int[4]
        {
        (int)UnitGroupMaskBit.Enemy | (int)UnitGroupMaskBit.Others,
        (int)UnitGroupMaskBit.Player | (int)UnitGroupMaskBit.Ally | (int)UnitGroupMaskBit.Others,
        (int)UnitGroupMaskBit.Enemy | (int)UnitGroupMaskBit.Others,
        (int)UnitGroupMaskBit.Player | (int)UnitGroupMaskBit.Enemy | (int)UnitGroupMaskBit.Ally
        };

        public static int[] Allies = new int[4]
        {
        (int)UnitGroupMaskBit.Player | (int)UnitGroupMaskBit.Ally,
        (int)UnitGroupMaskBit.Enemy,
        (int)UnitGroupMaskBit.Ally | (int)UnitGroupMaskBit.Player,
        (int)UnitGroupMaskBit.Others
        };

        public static int EnemyMask(UnitGroup group)
        {
            return Enemies[(int)group];
        }

        public static int AllyMask(UnitGroup group)
        {
            return Allies[(int)group];
        }

        public static bool ApplyMask(int mask, UnitGroup group)
        {
            return (mask & UnitGroupToMaskBit[(int)group]) > 0;
        }

        public static int Distance(Vector3Int a, Vector3Int b)
        {
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z));
        }
        
        public static bool IsPointWithinCollider(Collider collider, Vector3 point)
        {
            return (collider.ClosestPoint(point) - point).sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon;
        }
    }
    
    public static class IEnumeratorExtensions
    {
        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }

    [XLua.LuaCallCSharp]
    public class Databackend
    {
        const int MAX_MAP_SIZE = 16;
        private const int MAX_MAP_HEIGHT = 1;

        static private Databackend instance;
        static public Databackend GetSingleton()
        {
            if (instance == null)
            {
                instance = new Databackend();
            }
            return instance;
        }

        GridData[,,] map = new GridData[MAX_MAP_SIZE, MAX_MAP_HEIGHT, MAX_MAP_SIZE];
        bool[,,] visited = new bool[MAX_MAP_SIZE, MAX_MAP_HEIGHT, MAX_MAP_SIZE];
        public HashSet<MobData> allMobs { get; private set; } = new HashSet<MobData>();
        public Dictionary<GridEffect, List<Vector3Int>> allGridEffects { get; private set; } = new();
        public int mapSizeX, mapHeight, mapSizeZ;

        public event MobData.MobArgumentDelegate onMobAdded;
        public event MobData.MobArgumentDelegate onMobRemoved;

        public Spell testSpell;

        private Databackend()
        {
            for (int i = 0; i < MAX_MAP_SIZE; i++)
            {
                for (int j = 0; j < MAX_MAP_HEIGHT; j++)
                {
                    for (int k = 0; k < MAX_MAP_SIZE; k++)
                    {
                        map[i, j, k] = new GridData();
                    }
                }
            }

            mapSizeX = MAX_MAP_SIZE;
            mapHeight = MAX_MAP_HEIGHT;
            mapSizeZ = MAX_MAP_SIZE;
        }

        public GridData GetMap(int x, int y, int z)
        {
            if (x < 0 || x >= mapSizeX || y < 0 || y >= mapHeight || z < 0 || z >= mapSizeZ) { return null; }
            return map[x, y, z];
        }

        public GridData GetMap(Vector3Int pos) => GetMap(pos.x, pos.y, pos.z);
        
        public IEnumerator<Vector3Int> GetAllMapGridPositions()
        {
            for (int i = 0; i < mapSizeX; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    for (int k = 0; k < mapSizeZ; k++)
                    {
                        yield return new Vector3Int(i, j, k);
                    }
                }
            }
        }

        private void AddMob(MobData mob)
        {
            allMobs.Add(mob);
            onMobAdded?.Invoke(mob);
        }

        private void RemoveMob(MobData mob)
        {
            allMobs.Remove(mob);
            onMobRemoved?.Invoke(mob);
        }

        public void SetMob(int x, int y, int z, GridShape body, MobData mob)
        {
            foreach (Vector3Int p in body.shape)
            {
                map[x + p.x, y + p.y, z + p.z].mob = mob;
            }

            if (!allMobs.Contains(mob))
            {
                AddMob(mob);
            }
        }

        public void ClearMob(int x, int y, int z, GridShape body, MobData mob, bool remove = true)
        {
            foreach (Vector3Int p in body.shape)
            {
                if (map[x + p.x, y + p.y, z + p.z].mob == mob)
                {
                    map[x + p.x, y + p.y, z + p.z].mob = null;
                }
            }

            if (remove)
            {
                RemoveMob(mob);
            }
        }

        public void SetMob(Vector3Int pos, GridShape body, MobData mob)
        {
            SetMob(pos.x, pos.y, pos.z, body, mob);
        }

        public void ClearMob(Vector3Int pos, GridShape body, MobData mob, bool remove = true)
        {
            ClearMob(pos.x, pos.y, pos.z, body, mob, remove);
        }

        public void AddFx(GridEffect fx)
        {
            allGridEffects.Add(fx, new());
        }

        public void AddFxAt(GridEffect fx, Vector3Int pos)
        {
            if (!InMap(pos)) { return; }
            if(map[pos.x, pos.y, pos.z].effects.TryAdd(fx, fx))
            {
                allGridEffects[fx].Add(pos);
                if(map[pos.x, pos.y, pos.z].mob != null)
                {
                    fx.RegisterMob(map[pos.x, pos.y, pos.z].mob);
                }
            }
        }

        public void RemoveFx(GridEffect fx)
        {
            foreach (var p in allGridEffects[fx])
            {
                map[p.x, p.y, p.z].effects.Remove(fx);
                if (map[p.x, p.y, p.z].mob != null)
                {
                    fx.RemoveMob(map[p.x, p.y, p.z].mob);
                }
            }
            allGridEffects.Remove(fx);
        }

        Dictionary<GridEffect, bool> gridEffectChanges = new();
        public void MoveMob(Vector3Int from, Vector3Int to, MobData mob)
        {
            if (mob.initialized == false)
            {
                return;
            }
            
            gridEffectChanges.Clear();

            foreach(Vector3Int offset in mob.gridBody.shape)
            {
                Vector3Int from_o = from + offset;

                foreach (var fx in map[from_o.x, from_o.y, from_o.z].effects)
                {
                    gridEffectChanges.TryAdd(fx.Value, false);
                }
            }

            foreach (Vector3Int offset in mob.gridBody.shape)
            {
                Vector3Int to_o = to + offset;
                foreach (var fx in map[to_o.x, to_o.y, to_o.z].effects)
                {
                    if(gridEffectChanges.ContainsKey(fx.Value))
                    {
                        gridEffectChanges.Remove(fx.Value);
                    }
                    else
                    {
                        gridEffectChanges.Add(fx.Value, true);
                    }
                }
            }

            foreach (var fx in gridEffectChanges)
            {
                // Add
                if(fx.Value == true)
                {
                    fx.Key.RegisterMob(mob);
                }
                // Remove
                else
                {
                    fx.Key.RemoveMob(mob);
                }
            }

            ClearMob(from.x, from.y, from.z, mob.gridBody, mob, false);
            SetMob(to.x, to.y, to.z, mob.gridBody, mob);
        }

        // TODO: Modify me when implementing new renderer !!
        public Vector3Int GetGridPos(Vector3 pos)
        {
            // return new Vector3Int(Mathf.FloorToInt(pos.x), 0, Mathf.FloorToInt(pos.y));
            return new Vector3Int(Mathf.FloorToInt(pos.x), 0, Mathf.FloorToInt(pos.z));
        }

        // TODO: Modify me when implementing new renderer !!
        public Vector3 GridToWorldPos(Vector3Int gridPos)
        {
            // return new Vector3(gridPos.x, gridPos.z, 0) * 1.0f;
            return new Vector3(gridPos.x, 0, gridPos.z) * 1.0f;
        }

        public Vector3 GridToWorldPosCentered(Vector3Int gridPos)
        {
            return GridToWorldPos(gridPos) + new Vector3(0.5f, 0.5f, 0.5f);
        }
        
        public Vector3 GridToWorldPosCenteredGrounded(Vector3Int gridPos)
        {
            return GridToWorldPos(gridPos) + new Vector3(0.5f, 0.0f, 0.5f);
        }

        // TODO: Map border
        public GridShape.Direction GetDominantDirection(Vector3Int from, Vector3Int to)
        {
            Vector3Int diff = to - from;

            if(Mathf.Abs(diff.z) >= Mathf.Abs(diff.x) && diff.z >= 0)
            {
                return GridShape.Direction.Up;
            }

            if (Mathf.Abs(diff.z) >= Mathf.Abs(diff.x) && diff.z < 0)
            {
                return GridShape.Direction.Down;
            }

            if (Mathf.Abs(diff.z) <= Mathf.Abs(diff.x) && diff.x >= 0)
            {
                return GridShape.Direction.Right;
            }

            if (Mathf.Abs(diff.z) <= Mathf.Abs(diff.x) && diff.x < 0)
            {
                return GridShape.Direction.Left;
            }

            return GridShape.Direction.Up;
        }

        public void RecordDamageHeal(Consts.DamageHeal_Result result)
            => Globals.combatTracker.Record(result);

        public void RecordBuff(Consts.BuffEvents result)
            => Globals.combatTracker.Record(result);

        public IEnumerator DealDmgHeal(MobData target, Consts.DamageHeal_FrontEndInput input)
        {
            Consts.DamageHeal_Result result = new Consts.DamageHeal_Result();
            yield return new JumpIn(target.ReceiveDamage(input, result));

            //return true;
        }

        public bool InMap(Vector3Int pos)
        {
            return !((pos.x < 0) || (pos.x >= mapSizeX) || (pos.y < 0) || (pos.y >= mapHeight) || (pos.z < 0) || (pos.z >= mapSizeZ));
        }

        /// <summary>
        /// This method don't care if the mob will die while moving to target grid.
        /// In that case, you can select to move but your mob will die in the midways.
        /// </summary>
        /// <param name="mob">Mob that you want to check with. This method uses mob.data.position and mob.data.actionPoints as starting point.</param>
        /// <returns></returns>
        [Obsolete("Perhaps you should consider FindPathTo ... ? Idk, try avoid using this now, refactoring WIP for 3D grids")]
        public Dictionary<Vector3Int, float> GetMoveableGrids(MobData mob)
        {
            // TODO: detailed check
            return GetMoveableGrids(
                mob.Position,
                mob.actedThisTurn ? 0 : mob.MoveRangeLeft,
                Mathf.FloorToInt(mob.actionPoints),
                mob.baseDescriptor.movementType);
        }
        
        [Obsolete("Perhaps you should consider FindPathTo ... ? Idk, try avoid using this now, refactoring WIP for 3D grids")]
        public Dictionary<Vector3Int, float> GetMoveableGrids(Vector3Int startPos, int freeRange = 3, int exMove = 5,
            BaseMobDescriptorSO.MovementType moveType = BaseMobDescriptorSO.MovementType.Walk)
        {
            Dictionary<Vector3Int, float> result = new();
            Queue<KeyValuePair<Vector3Int, int>> BFSQueue = new Queue<KeyValuePair<Vector3Int, int>>();
            System.Array.Clear(visited, 0, visited.Length);

            Vector3Int[] dirc = new Vector3Int[] { new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1), new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0) };

            BFSQueue.Enqueue(new KeyValuePair<Vector3Int, int>(
                startPos,
                freeRange + exMove)
            );
            visited[startPos.x, startPos.y, startPos.z] = true;

            while (BFSQueue.Count > 0)
            {
                var current = BFSQueue.Dequeue();
                result.Add(current.Key, current.Value);

                foreach (var dir in dirc)
                {
                    var next = current.Key + dir;
                    if (!InMap(next)) { continue; }
                    if (visited[next.x, next.y, next.z] == false && current.Value > 0)
                    {
                        visited[next.x, next.y, next.z] = true;
                        BFSQueue.Enqueue(new KeyValuePair<Vector3Int, int>(next, current.Value - 1));
                    }
                }
            }

            return result;
        }

        // TODO: Implement this
        public bool IsMoveable(GridData grid, MobData.MovementType type, out int cost)
        {
            cost = 1;

            if (type == MobData.MovementType.Fly)
            {
                return true;
            }
            
            return grid.mob == null;
        }

        public delegate bool IsGridValidFunc(Vector3Int pos, GridData data);
        public delegate bool IsMobValidFunc(MobData mob);

        public HashSet<Vector3Int> GetGridsWithMob(IsMobValidFunc mobFilter, IsGridValidFunc gridFilter)
        {
            HashSet<Vector3Int> result = new HashSet<Vector3Int>();

            foreach (var mob in allMobs)
            {
                if (mobFilter == null || mobFilter(mob))
                {
                    Vector3Int pivot = mob.Position;
                    foreach (var grid in mob.gridBody.shape)
                    {
                        Vector3Int current = pivot + grid;
                        GridData data = map[current.x, current.y, current.z];

                        if (gridFilter == null || gridFilter(current, data))
                        {
                            result.Add(current);
                        }
                    }
                }
            }

            return result;
        }

        public struct GridBFSKeys : IComparable
        {
            public Vector3Int position;
            public int distance;

            public int CompareTo(object obj)
            {
                return distance - ((GridBFSKeys)obj).distance;
            }

            public GridBFSKeys(Vector3Int p, int d)
            {
                this.position = p;
                this.distance = d;
            }

            public GridBFSKeys(int x, int y, int d)
            {
                this.position = new Vector3Int(x, y);
                this.distance = d;
            }
        }

        public enum Direction
        {
            // X+, X-
            Right = 0,
            Left = 1,

            // Y+, Y-
            Up = 2,
            Down = 3
        }

        public readonly Vector3Int[] dirc_dxyz =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),
        };

        public readonly Direction[] possibleDirections =
        {
            Direction.Right,
            Direction.Left,
            Direction.Up,
            Direction.Down,
        };

        public GridPath FindPathTo(Vector3Int from, Vector3Int to, MobData.MovementType movementType = MobData.MovementType.Walk, int maxDistance = -1)
        {
            C5.IntervalHeap<GridBFSKeys> searchedGrids = new C5.IntervalHeap<GridBFSKeys>();
            Dictionary<Vector3Int, Vector3Int> prevGrid = new Dictionary<Vector3Int, Vector3Int>();

            searchedGrids.Add(new GridBFSKeys(from, 0));
            prevGrid.Add(from, from);

            while(!searchedGrids.IsEmpty)
            {
                var curr = searchedGrids.DeleteMin();

                if (curr.position == to)
                {
                    // Found a path
                    GridPath path = new GridPath();
                    path.path = new List<Vector3Int>();

                    Vector3Int pathCurr = curr.position;
                    while(prevGrid[pathCurr] != pathCurr)
                    {
                        path.path.Add(pathCurr);
                        pathCurr = prevGrid[pathCurr];
                    }

                    path.path.Reverse();
                    return path;
                }

                foreach (var d in possibleDirections)
                {
                    var newPos = curr.position + dirc_dxyz[(int)d];
                    if(!prevGrid.ContainsKey(newPos))
                    {
                        // Get cost of grid
                        // TODO: IsMoveable might get stuck with >1x1 gridBodies
                        if(InMap(newPos) && IsMoveable(GetMap(newPos.x, newPos.y, newPos.z), movementType, out int cost))
                        {
                            if(maxDistance < 0 || (curr.distance + cost) <= maxDistance)
                            {
                                // TODO: Record path
                                searchedGrids.Add(new GridBFSKeys(newPos, curr.distance + cost));
                                prevGrid.Add(newPos, curr.position);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public bool IsPathValid(MobData mob, GridPath path)
        {
            // TODO: detailed check

            return path.path.Count <= mob.actionPoints;
        }

        public bool CanGridPlaceMob(Vector3Int center, GridShape body)
        {
            if (body != null && body.shape.Count > 1)
            {
                throw new NotImplementedException();
            }

            return map[center.x, center.y, center.z].mob == null;
        }

        public Vector3Int FindNearestEmptyGrid(Vector3Int center) => FindNearestEmptyGrid(center, null);

        public Vector3Int FindNearestEmptyGrid(Vector3Int center, GridShape body)
        {
            if (!InMap(center)) { return -Vector3Int.one; }
            if(CanGridPlaceMob(center, body)) { return center; }

            int start = UnityEngine.Random.Range(0, 4);

            for (int distance = 1; distance < 5; distance++)
            {
                int x = 0;
                int y = distance - x;

                for(int dirc = 0; dirc < 4; dirc += 1)
                {
                    for(int p = 0; p < distance; p++)
                    {
                        // Compute x, y
                        switch((dirc + start) % 4)
                        {
                            case 0:
                                x = p;
                                y = distance - p;
                                break;
                            case 1:
                                x = distance - p;
                                y = -p;
                                break;
                            case 2:
                                x = -p;
                                y = p - distance;
                                break;
                            case 3:
                                x = p - distance;
                                y = p;
                                break;
                            default:
                                x = 0;
                                y = 0;
                                break;
                        }

                        Vector3Int pos = center + new Vector3Int(x, y);
                        if (InMap(pos) && CanGridPlaceMob(pos, body)) 
                        { 
                            return pos;
                        }
                    }
                }
            }

            return -Vector3Int.one;
        }

        public int Distance(Vector3Int a, Vector3Int b)
        {
            var tmp = (a - b);
            return Mathf.Abs(tmp.x) + Mathf.Abs(tmp.y) + Mathf.Abs(tmp.z);
        }

        public List<MobData> GetAllMobs()
        {
            return allMobs.ToList();
        }

        #region BackendState-Undos

        private List<BackendState> allStates = new();
        
        public void RegisterState(BackendState state)
        {
            Debug.LogWarning($"DataBackend.RegisterState is not implemented. Incoming state of type: {state.GetType()}");
            
            // TODO
            allStates.Add(state);
        }

        public void AimOnTarget(MobData targetMob)
        {
            // TODO: Enemy?
            var mobs = Globals.backend.allMobs.Where(x => x.unitGroup == Consts.UnitGroup.Player);
            SpellTarget target = new SpellTarget(targetMob.Position);
            foreach (MobData mob in mobs)
            {
                if (mob.mainWeapon?.GetRegularAttackSpell()?.data.CheckWithTargets(mob, target) ?? false)
                {
                    mob.lastTurnTarget = targetMob;
                }
            }
        }

        #endregion
    }
}
