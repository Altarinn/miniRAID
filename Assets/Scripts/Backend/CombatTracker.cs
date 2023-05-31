using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

using miniRAID;

using Records = System.Collections.Generic.Dictionary<miniRAID.MobData, System.Collections.Generic.Dictionary<string, miniRAID.CombatTracker.RecordEntry>>;

namespace miniRAID
{
    public class CombatTracker
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

        private Logger combatLog = new Logger("miniRAID.combat.log");

        List<Records> pastRecords;
        Records records = new();
        int _turns = 0;

        public miniRAID.UIElements.CombatStatsController ui;
        
        public int Turns
        {
            get { return _turns; }
            set { _turns = value; UpdateUI(); combatLog.Log($"回合{_turns}"); }
        }

        public void Reset()
        {
            pastRecords.Add(records);
            records = new();
        }

        public void Record(Consts.DamageHeal_Result result)
        {
            // No renderer, maybe we are in a detached thread of computation (not what player is looking at)
            if (result.target.mobRenderer == null) { return; }

            string damageInfo =
                $"{result.source.nickname} 的 {result.Name} ";
            
            if (result.isAvoid)
            {
                damageInfo +=
                    $"没有击中 {result.target.nickname}。";
            }
            else
            {
                damageInfo +=
                    $"对 {result.target.nickname} 造成了 {result.value} " +
                    $"点 {result.type} {(result.type == Consts.Elements.Heal ? "" : "伤害")}" +
                    $"{(result.isCrit ? "（暴击）" : "")}。";
            }
            
            combatLog.Log(damageInfo + $"生命值：{result.target.health}");
            Globals.debugMessage.Instance.Message(damageInfo);

            // TODO: make a popup manager
            // {result.Name} 
            if (result.popup)
            {
                if (result.isAvoid)
                {
                    Globals.popupMgr.Instance.Popup("MISS", result.target.mobRenderer.transform.position);
                }
                else
                {
                    Globals.popupMgr.Instance.Popup(
                        $"{result.value.ToString()}{(result.isCrit ? "!" : "")}", 
                        result.target.mobRenderer.transform.position);
                }
            }
            
            // Record them to database
            MobData src = result.source;
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

        public void Record(Consts.BuffEvents buffEvent)
        {
            // No renderer, maybe we are in a detached thread of computation (not what player is looking at)
            if (buffEvent.buff.parentMob.mobRenderer == null) { return; }
            
            Vector2 popupPosition = buffEvent.buff.parentMob.Position + new Vector2(0.5f, 0.8f);
            switch (buffEvent.eventType)
            {
                case Consts.BuffEventType.Attached:
                    combatLog.Log($"{buffEvent.buff.parentMob.nickname}" +
                                  $" 获得了来自 {buffEvent.buff.source?.nickname} 的 " +
                                  $"{buffEvent.buff.name}");
                    Globals.popupMgr.Instance.Popup($"+{buffEvent.buff.name}", popupPosition);
                    break;
                case Consts.BuffEventType.Stacked:
                case Consts.BuffEventType.Refreshed:
                    combatLog.Log($"{buffEvent.buff.parentMob.nickname}" +
                                  $" 重新获得了来自 {buffEvent.buff.source?.nickname} 的 " +
                                  $"{buffEvent.buff.name}");
                    Globals.popupMgr.Instance.Popup($"o{buffEvent.buff.name}", popupPosition);
                    break;
                case Consts.BuffEventType.Removed:
                    combatLog.Log($"{buffEvent.buff.parentMob.nickname}" +
                                  $" 失去了来自 {buffEvent.buff.source?.nickname} 的 " +
                                  $"{buffEvent.buff.name}");
                    Globals.popupMgr.Instance.Popup($"-{buffEvent.buff.name}", popupPosition);
                    break;
            }
        }

        public void Record(Consts.KillEvent killEvent)
        {
            // No renderer, maybe we are in a detached thread of computation (not what player is looking at)
            if (killEvent.info.target.mobRenderer == null) { return; }
            
            var info = killEvent.info;
            string message =
                $"{info.source?.nickname} 的 {info.sourceAction?.data.ActionName} 击杀了 {info.target.nickname} !";
            combatLog.Log(message);
            Globals.debugMessage.Instance.Message(message);
        }

        public void Record(Consts.TrackerActionEvent actionEvent)
        {
            string message = $"{actionEvent.action.parentMob.nickname} 施放了 {actionEvent.action.data.ActionName}, 目标：";
            foreach (var p in actionEvent.target.targetPos)
            {
                message += $"({p.x}, {p.y}) ";
            }
            combatLog.Log(message);
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
