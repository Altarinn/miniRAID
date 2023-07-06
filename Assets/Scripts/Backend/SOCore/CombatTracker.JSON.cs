using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = System.Object;

namespace miniRAID
{
    
    
    public partial class CombatTracker
    {
        private struct FullJSONData
        {
            public List<List<CombatJSONEntry>> events;
            public List<MobEntry> mobs;
        }
        
        private Dictionary<miniRAID.MobData, int> mobID;
        private FullJSONData fullJSONData;
        private List<CombatJSONEntry> currentTurnJSONdata;

        public class TurnSummary
        {
            [TableColumnWidth(40, Resizable = false)]
            [DisplayAsString]
            public int turn;
            
            [DisplayAsString]
            public float DmgDealt;
            
            [DisplayAsString]
            public float DmgTaken;
            
            [DisplayAsString]
            public float Healing;
        }

        public List<TurnSummary> turnSummaries = new();

        public enum CombatJSONEntryType
        {
            MobAdded,
            MobRemoved,
            ListenerAdded,
            ListenerRemoved,
            PerformAction,
            EffectTriggered,
            DamageHeal,
            EnergyGain,
            Phase,
            Turn,
        }
        
        public struct CombatJSONEntry
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public CombatJSONEntryType type;
            public Object entry;
        }

        public struct MobAddRemovalEntry
        {
            public int mob;
            public string reason;
        }
        
        public struct MobEntry
        {
            public string nickname;
            
            [JsonConverter(typeof(StringEnumConverter))]
            public Consts.UnitGroup group;
        }

        public struct ListenerAddRemovalEntry
        {
            public int targetMob;
            public string listenerName;
            public int listenerSourceMob;
        }

        public struct CostEntry
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public Cost.Type type;
            public float value;
        }

        public struct ActionPerformedEntry
        {
            public int sourceMob;
            public string actionName;
            
            public string[] flags;
            public float power, auxPower, hit, crit;
            
            public Vector3Int[] spellTargets;
            public int[] capturedMobs; // getMap(spellTargets[i]).mob at the time the action performed

            public CostEntry[] costs;
        }

        public struct DamageHealEntry
        {
            public int sourceMob;
            public int targetMob;

            public int value;
            public int overdeal;
            
            [JsonConverter(typeof(StringEnumConverter))]
            public Consts.Elements type;

            public bool isCrit;
            public bool isAvoid;
            public bool isBlock;

            public bool isAction;
            public string actionOrListenerName;
            public string[] flags;
        }

        public struct PhaseEntry
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public Consts.UnitGroup group;
        }
        
        public struct TurnEntry
        {
            public int turn;
        }

        private string[] FlagsToStringArray<T>(T type) where T : Enum
        {
            List<string> result = new();
            foreach (T i in Enum.GetValues(typeof(T)))
            {
                // Ugly ...
                if (((int)(object)(type) & (int)(object)(i)) > 0)
                {
                    result.Add(i.ToString());
                }
            }

            return result.ToArray();
        }

        public void InitJSON()
        {
            fullJSONData = new FullJSONData()
            {
                events = new List<List<CombatJSONEntry>>(),
                mobs = new List<MobEntry>(),
            };
            mobID = new();

            Globals.backend.onMobAdded += OnMobAdded;
        }

        public void OnMobAdded(MobData mob)
        {
            if (!mobID.ContainsKey(mob))
            {
                mobID.Add(mob, mobID.Count);
                fullJSONData.mobs.Add(new MobEntry()
                {
                    nickname = mob.nickname,
                    group = mob.unitGroup,
                });
            }
        }

        public void OnNextTurn(int turn)
        {
            if (turn > fullJSONData.events.Count)
            {
                currentTurnJSONdata = new List<CombatJSONEntry>
                {
                    new CombatJSONEntry()
                    {
                        type = CombatJSONEntryType.Turn,
                        entry = new TurnEntry(){turn = turn},
                    }
                };

                fullJSONData.events.Add(currentTurnJSONdata);
            }
            
            turnSummaries.Add(new TurnSummary());
            turnSummaries[^1].turn = turn;
        }

        public void RecordJSON_DamageHeal(Consts.DamageHeal_Result result)
        {
            currentTurnJSONdata.Add(new CombatJSONEntry()
            {
                type = CombatJSONEntryType.DamageHeal,
                entry = new DamageHealEntry()
                {
                    sourceMob = mobID[result.source],
                    targetMob = mobID[result.target],
                    
                    value = result.value,
                    overdeal = result.overdeal,
                    type = result.type,
                    
                    isCrit = result.isCrit,
                    isAvoid = result.isAvoid,
                    isBlock = result.isBlock,
                    
                    isAction = result.IsAction,
                    actionOrListenerName = result.Name,
                    
                    flags = FlagsToStringArray(result.flags),
                }
            });

            if (result.type != Consts.Elements.Heal)
            {
                if (result.source.unitGroup == Consts.UnitGroup.Player)
                {
                    turnSummaries[^1].DmgDealt += result.value;
                }
                else if (result.target.unitGroup == Consts.UnitGroup.Player)
                {
                    turnSummaries[^1].DmgTaken += result.value;
                }
            }
            else
            {
                if (result.source.unitGroup == Consts.UnitGroup.Player)
                {
                    turnSummaries[^1].Healing += result.value;
                }
            }
        }

        public void ExportJSON()
        {
            string jsonString = JsonConvert.SerializeObject(fullJSONData, Formatting.Indented);
            Debug.Log(jsonString);
#if UNITY_WEBGL && !(UNITY_EDITOR)
            // throw new NotImplementedException();
#else
            File.WriteAllText($"CombatLogs/JSON/{DateTime.Now.ToString("yy-MM-dd--HH-mm-ss")}-miniRAIDCombat.json", jsonString);
#endif
        }
    }
}