using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using miniRAID.Spells;
using miniRAID.Buff;
using System;
using System.Linq;
using miniRAID.Backend;

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
        public List<Vector2Int> path = new List<Vector2Int>();
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
        }
        
        [Flags]
        // TODO: Buff dispel types?
        public enum BuffFlags
        {
            // 无法被驱散
            CannotBeDispelled = 1 << 1,
        }

        public struct DamageHeal_FrontEndInput
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
            public int Id => IsAction ? sourceAction.data.Id : sourceBuff.data.Id;
            public string Name => IsAction ? sourceAction.data.ActionName : sourceBuff.data.name;
            public string Description => IsAction ? sourceAction.data.Description : "No description";

            public bool popup;
        }

        public class DamageHeal_FrontEndInput_ByRef
        {
            public DamageHeal_FrontEndInput value;

            public DamageHeal_FrontEndInput_ByRef(DamageHeal_FrontEndInput val)
            {
                value = val;
            }
        }

        public static bool IsHeal(DamageHeal_FrontEndInput input)
        {
            return input.type == Elements.Heal;
        }

        public struct DamageHeal_ComputedRates
        {
            public int value;
            public float hit, crit;
        }

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
            public int Id => NoInfo ? -1 : (IsAction ? sourceAction.data.Id : sourceBuff.data.Id);
            public string Name => NoInfo ? "<NULL ACTION>" : (IsAction ? sourceAction.data.ActionName : sourceBuff.data.name);
            public string Description => NoInfo ? "<NULL ACTION>" : (IsAction ? sourceAction.data.Description : "No description");

            public bool popup;
        }

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
        public struct BaseStats
        {
            public dNumber VIT, STR, MAG, INT, DEX, TEC;
        }

        // TODO: Determine our values

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
        public static float HitRangePerLevel = 5;
        public static float BaseHit = 0.5f;
        public static float MaxHitAcc = 1000000.0f;
        
        // Same but for critical strikes.
        public static float CritRangePerLevel = 7;
        public static float BaseCrit = -0.1f;

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
        
        [Serializable]
        public struct BattleStats
        {
            public AllTypes<float> exResist;
            public AllTypes<float> exDamage;
        }

        public enum UnitGroup
        {
            Player = 0,
            Enemy = 1,
            Ally = 2,
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

        public static int Distance(Vector2Int a, Vector2Int b)
        {
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
        }
    }

    [XLua.LuaCallCSharp]
    public class Databackend
    {
        const int MAX_MAP_SIZE = 16;

        static private Databackend instance;
        static public Databackend GetSingleton()
        {
            if (instance == null)
            {
                instance = new Databackend();
            }
            return instance;
        }

        GridData[,] map = new GridData[MAX_MAP_SIZE, MAX_MAP_SIZE];
        bool[,] visited = new bool[MAX_MAP_SIZE, MAX_MAP_SIZE];
        public HashSet<MobData> allMobs { get; private set; } = new HashSet<MobData>();
        public Dictionary<GridEffect, List<Vector2Int>> allGridEffects { get; private set; } = new();
        public int mapWidth, mapHeight;

        public Spell testSpell;

        private Databackend()
        {
            for (int i = 0; i < MAX_MAP_SIZE; i++)
            {
                for (int j = 0; j < MAX_MAP_SIZE; j++)
                {
                    map[i, j] = new GridData();
                }
            }

            mapWidth = MAX_MAP_SIZE;
            mapHeight = MAX_MAP_SIZE;
        }

        public GridData getMap(int x, int y)
        {
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight) { return null; }
            return map[x, y];
        }

        public void SetMob(int x, int y, GridShape body, MobData mob)
        {
            foreach (Vector2Int p in body.shape)
            {
                map[x + p.x, y + p.y].mob = mob;
            }

            if (!allMobs.Contains(mob))
            {
                allMobs.Add(mob);
            }
        }

        public void ClearMob(int x, int y, GridShape body, MobData mob, bool remove = true)
        {
            foreach (Vector2Int p in body.shape)
            {
                if (map[x + p.x, y + p.y].mob == mob)
                {
                    map[x + p.x, y + p.y].mob = null;
                }
            }

            if (remove)
            {
                allMobs.Remove(mob);
            }
        }

        public void SetMob(Vector2Int pos, GridShape body, MobData mob)
        {
            SetMob(pos.x, pos.y, body, mob);
        }

        public void ClearMob(Vector2Int pos, GridShape body, MobData mob, bool remove = true)
        {
            ClearMob(pos.x, pos.y, body, mob, remove);
        }

        public void AddFx(GridEffect fx)
        {
            allGridEffects.Add(fx, new());
        }

        public void AddFxAt(GridEffect fx, Vector2Int pos)
        {
            if (!InMap(pos)) { return; }
            if(map[pos.x, pos.y].effects.TryAdd(fx, fx))
            {
                allGridEffects[fx].Add(pos);
                if(map[pos.x, pos.y].mob != null)
                {
                    fx.RegisterMob(map[pos.x, pos.y].mob);
                }
            }
        }

        public void RemoveFx(GridEffect fx)
        {
            foreach (var p in allGridEffects[fx])
            {
                map[p.x, p.y].effects.Remove(fx);
                if (map[p.x, p.y].mob != null)
                {
                    fx.RemoveMob(map[p.x, p.y].mob);
                }
            }
            allGridEffects.Remove(fx);
        }

        Dictionary<GridEffect, bool> gridEffectChanges = new();
        public void MoveMob(Vector2Int from, Vector2Int to, MobData mob)
        {
            if (mob.initialized == false)
            {
                return;
            }
            
            gridEffectChanges.Clear();

            foreach(Vector2Int offset in mob.gridBody.shape)
            {
                Vector2Int from_o = from + offset;

                foreach (var fx in map[from_o.x, from_o.y].effects)
                {
                    gridEffectChanges.TryAdd(fx.Value, false);
                }
            }

            foreach (Vector2Int offset in mob.gridBody.shape)
            {
                Vector2Int to_o = to + offset;
                foreach (var fx in map[to_o.x, to_o.y].effects)
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

            ClearMob(from.x, from.y, mob.gridBody, mob, false);
            SetMob(to.x, to.y, mob.gridBody, mob);
        }

        public Vector2Int GetGridPos(Vector2 pos)
        {
            return new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        }

        public Vector2 GridToWorldPos(Vector2Int gridPos)
        {
            return (Vector2)gridPos * 1.0f;
        }

        // TODO: Map border
        public GridShape.Direction GetDominantDirection(Vector2Int from, Vector2Int to)
        {
            Vector2Int diff = to - from;

            if(Mathf.Abs(diff.y) >= Mathf.Abs(diff.x) && diff.y >= 0)
            {
                return GridShape.Direction.Up;
            }

            if (Mathf.Abs(diff.y) >= Mathf.Abs(diff.x) && diff.y < 0)
            {
                return GridShape.Direction.Down;
            }

            if (Mathf.Abs(diff.y) <= Mathf.Abs(diff.x) && diff.x >= 0)
            {
                return GridShape.Direction.Right;
            }

            if (Mathf.Abs(diff.y) <= Mathf.Abs(diff.x) && diff.x < 0)
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

        public bool InMap(Vector2Int pos)
        {
            return !((pos.x < 0) || (pos.x >= mapWidth) || (pos.y < 0) || (pos.y >= mapHeight));
        }

        /// <summary>
        /// This method don't care if the mob will die while moving to target grid.
        /// In that case, you can select to move but your mob will die in the midways.
        /// </summary>
        /// <param name="mob">Mob that you want to check with. This method uses mob.data.position and mob.data.actionPoints as starting point.</param>
        /// <returns></returns>
        public HashSet<Vector2Int> GetMoveableGrids(MobData mob)
        {
            // TODO: detailed check

            HashSet<Vector2Int> result = new HashSet<Vector2Int>();
            Queue<KeyValuePair<Vector2Int, int>> BFSQueue = new Queue<KeyValuePair<Vector2Int, int>>();
            System.Array.Clear(visited, 0, visited.Length);

            Vector2Int[] dirc = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };

            BFSQueue.Enqueue(new KeyValuePair<Vector2Int, int>(mob.Position, Mathf.RoundToInt(mob.actionPoints * 100)));
            visited[mob.Position.x, mob.Position.y] = true;

            while (BFSQueue.Count > 0)
            {
                var current = BFSQueue.Dequeue();
                result.Add(current.Key);

                foreach (var dir in dirc)
                {
                    var next = current.Key + dir;
                    if (!InMap(next)) { continue; }
                    if (visited[next.x, next.y] == false && current.Value > 0)
                    {
                        visited[next.x, next.y] = true;
                        BFSQueue.Enqueue(new KeyValuePair<Vector2Int, int>(next, current.Value - 50));
                    }
                }
            }

            return result;
        }

        // TODO: Implement this
        public bool IsMoveable(GridData grid, MobData.MovementType type, out int cost)
        {
            cost = 1;
            return true;
        }

        public delegate bool IsGridValidFunc(Vector2Int pos, GridData data);
        public delegate bool IsMobValidFunc(MobData mob);

        public HashSet<Vector2Int> GetGridsWithMob(IsMobValidFunc mobFilter, IsGridValidFunc gridFilter)
        {
            HashSet<Vector2Int> result = new HashSet<Vector2Int>();

            foreach (var mob in allMobs)
            {
                if (mobFilter == null || mobFilter(mob))
                {
                    Vector2Int pivot = mob.Position;
                    foreach (var grid in mob.gridBody.shape)
                    {
                        Vector2Int current = pivot + grid;
                        GridData data = map[current.x, current.y];

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
            public Vector2Int position;
            public int distance;

            public int CompareTo(object obj)
            {
                return distance - ((GridBFSKeys)obj).distance;
            }

            public GridBFSKeys(Vector2Int p, int d)
            {
                this.position = p;
                this.distance = d;
            }

            public GridBFSKeys(int x, int y, int d)
            {
                this.position = new Vector2Int(x, y);
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

        public readonly Vector2Int[] dirc_dxy =
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
        };

        public readonly int[] dirc_dx = { 1, -1, 0, 0 };
        public readonly int[] dirc_dy = { 0, 0, 1, -1 };

        public readonly Direction[] possibleDirections =
        {
            Direction.Right,
            Direction.Left,
            Direction.Up,
            Direction.Down,
        };

        public GridPath FindPathTo(Vector2Int from, Vector2Int to, MobData.MovementType movementType = MobData.MovementType.Walk, int maxDistance = -1)
        {
            C5.IntervalHeap<GridBFSKeys> searchedGrids = new C5.IntervalHeap<GridBFSKeys>();
            Dictionary<Vector2Int, Vector2Int> prevGrid = new Dictionary<Vector2Int, Vector2Int>();

            searchedGrids.Add(new GridBFSKeys(from, 0));
            prevGrid.Add(from, from);

            while(!searchedGrids.IsEmpty)
            {
                var curr = searchedGrids.DeleteMin();

                if (curr.position == to)
                {
                    // Found a path
                    GridPath path = new GridPath();
                    path.path = new List<Vector2Int>();

                    Vector2Int pathCurr = curr.position;
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
                    var newPos = curr.position + dirc_dxy[(int)d];
                    if(!prevGrid.ContainsKey(newPos))
                    {
                        // Get cost of grid
                        if(InMap(newPos) && IsMoveable(getMap(newPos.x, newPos.y), movementType, out int cost))
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

        public bool CanGridPlaceMob(Vector2Int center, GridShape body)
        {
            if (body != null && body.shape.Count > 1)
            {
                throw new NotImplementedException();
            }

            return map[center.x, center.y].mob == null;
        }

        public Vector2Int FindNearestEmptyGrid(Vector2Int center) => FindNearestEmptyGrid(center, null);

        public Vector2Int FindNearestEmptyGrid(Vector2Int center, GridShape body)
        {
            if (!InMap(center)) { return -Vector2Int.one; }
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

                        Vector2Int pos = center + new Vector2Int(x, y);
                        if (InMap(pos) && CanGridPlaceMob(pos, body)) 
                        { 
                            return pos;
                        }
                    }
                }
            }

            return -Vector2Int.one;
        }

        public int Distance(Vector2Int a, Vector2Int b)
        {
            var tmp = (a - b);
            return Mathf.Abs(tmp.x) + Mathf.Abs(tmp.y);
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

        #endregion
    }
}
