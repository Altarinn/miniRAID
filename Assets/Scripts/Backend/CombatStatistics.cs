using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

using miniRAID;

using Records = System.Collections.Generic.Dictionary<miniRAID.MobData, System.Collections.Generic.Dictionary<string, miniRAID.CombatStatistics.RecordEntry>>;

namespace miniRAID
{
    public class CombatStatistics
    {
        public class RecordEntry
        {
            public int totalDamage;
            public int totalHeal;
            public int numCasted;

            public RecordEntry()
            {
                totalDamage = 0;
                totalHeal = 0;
                numCasted = 0;
            }
        }

        List<Records> pastRecords;
        Records records = new();
        int _turns = 0;

        public miniRAID.UIElements.CombatStatsController ui;
        
        public int Turns
        {
            get { return _turns; }
            set { _turns = value; UpdateUI(); }
        }

        public void Reset()
        {
            pastRecords.Add(records);
            records = new();
        }

        public void Record(Consts.DamageHeal_Result result)
        {
            MobData src = result.source.data;
            if(!records.ContainsKey(src))
            {
                records.Add(src, new Dictionary<string, RecordEntry>());
            }

            string actName = result.Name;
            if (!records[src].ContainsKey(actName))
            {
                records[src].Add(actName, new RecordEntry());
            }

            records[src][actName].numCasted++;

            if(result.type == Consts.Elements.Heal)
            {
                records[src][actName].totalHeal += result.value;
            }
            else
            {
                records[src][actName].totalDamage += result.value;
            }

            UpdateUI();
        }

        public void SetUI(UIElements.CombatStatsController ui)
        {
            this.ui = ui;
        }

        // DEBUG ONLY
        public void UpdateUI()
        {
            ui.UpdateInfo(GetDamageStr(), GetHealingStr());
        }

        public string GetDamageStr()
        {
            StringBuilder builder = new();

            foreach (MobData mob in records.Keys)
            {
                if( mob.unitGroup == Consts.UnitGroup.Enemy 
                 || mob.unitGroup == Consts.UnitGroup.Others)
                {
                    continue;
                }

                int dmg = 0;
                foreach (var name in records[mob].Keys)
                {
                    dmg += records[mob][name].totalDamage;
                }

                if (dmg == 0) { continue; }

                builder.AppendLine($"{mob.nickname}: {dmg} ({dmg / (float)Turns:0.#})");
            }

            return builder.ToString().TrimEnd('\n', '\r');
        }

        public string GetHealingStr()
        {
            StringBuilder builder = new();

            foreach (MobData mob in records.Keys)
            {
                if (mob.unitGroup == Consts.UnitGroup.Enemy
                 || mob.unitGroup == Consts.UnitGroup.Others)
                {
                    continue;
                }

                int heal = 0;
                foreach (var name in records[mob].Keys)
                {
                    heal += records[mob][name].totalHeal;
                }

                if (heal == 0) { continue; }

                builder.AppendLine($"{mob.nickname}: {heal} ({heal / (float)Turns:0.#})");
            }

            return builder.ToString().TrimEnd('\n', '\r');
        }
    }
}
