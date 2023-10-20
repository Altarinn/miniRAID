using UnityEngine;
using UnityEngine.UIElements;

namespace miniRAID.UIElements
{
    public class MobInfoController
    {
        private VisualElement masterElem, activeSkillContainer, passiveSkillContainer, effectContainer;
        public VisualTreeAsset skillInfoTemplate;

        private Label name, racejob, ap, sp, hp, bp, def, res, phb, mab, hit, dodge, aprecover, crit, overload, exrange;
        
        public MobInfoController(VisualElement elem)
        {
            masterElem = elem;
            
            skillInfoTemplate = Resources.Load<VisualTreeAsset>("UI/SkillRow");
            activeSkillContainer = elem.Q("Active-Contents");
            passiveSkillContainer = elem.Q("Passive-Contents");
            effectContainer = elem.Q("Effect-Contents");

            name = elem.Query("title").Children<Label>("name");
            racejob = elem.Query<Label>("racejob");
            
            ap = elem.Query("AttackPower-row").Children<Label>("row-value");
            sp = elem.Query("SpellPower-row").Children<Label>("row-value");
            hp = elem.Query("HealPower-row").Children<Label>("row-value");
            bp = elem.Query("BuffPower-row").Children<Label>("row-value");
            
            def = elem.Query("Defense-row").Children<Label>("row-value");
            res = elem.Query("Resist-row").Children<Label>("row-value");
            phb = elem.Query("PhyBlock-row").Children<Label>("row-value");
            mab = elem.Query("MagBlock-row").Children<Label>("row-value");
            
            hit = elem.Query("HitAcc-row").Children<Label>("row-value");
            dodge = elem.Query("Dodge-row").Children<Label>("row-value");
            aprecover = elem.Query("APRecover-row").Children<Label>("row-value");
            
            crit = elem.Query("Critical-row").Children<Label>("row-value");
            overload = elem.Query("Overload-row").Children<Label>("row-value");
            exrange = elem.Query("ExRange-row").Children<Label>("row-value");
        }
        
        public void Show(MobData mob)
        {
            masterElem.visible = true;
            
            // Stats
            name.text = mob.nickname;
            racejob.text = $"{mob.race} / {mob.job}";
            
            ap.text = Mathf.CeilToInt(mob.attackPower).ToString();
            sp.text = Mathf.CeilToInt(mob.spellPower).ToString();
            hp.text = Mathf.CeilToInt(mob.healPower).ToString();
            bp.text = Mathf.CeilToInt(mob.buffPower).ToString();
            
            def.text = Mathf.CeilToInt(mob.defense).ToString();
            res.text = Mathf.CeilToInt(mob.spDefense).ToString();
            phb.text = "0";
            mab.text = "0";
            
            hit.text = $"{(100.0f * Consts.GetHitRate(mob.hitAcc, EnemyMobDescriptorSO.BaseDodgePerLevel * mob.level, mob.level)).ToString("F2")}% ({Mathf.CeilToInt(mob.hitAcc).ToString()})";
            dodge.text = $"{(100.0f * (1.0f - Consts.GetHitRate(EnemyMobDescriptorSO.BaseHitAccPerLevel * mob.level, mob.dodge, mob.level))).ToString("F2")}% ({Mathf.CeilToInt(mob.dodge).ToString()})";
            aprecover.text = mob.apRecovery.ToString("F2");

            crit.text =
                $"{(100.0f * Consts.GetCriticalRate(mob.crit, EnemyMobDescriptorSO.BaseAntiCritPerLevel * mob.level, mob.level)).ToString("F2")}% ({Mathf.CeilToInt(mob.crit).ToString()})";
            overload.text = "0";
            exrange.text = "0";

            // Skill and effects
            activeSkillContainer.Clear();
            passiveSkillContainer.Clear();
            effectContainer.Clear();
            
            foreach (var skill in mob.actions)
            {
                var skillInfo = skillInfoTemplate.CloneTree();
                skillInfo.Q<Label>("name").text = $"Lv{(skill).level + 1} {(skill).data.ActionName}";
                skillInfo.Q<Label>("info").text = skill.Tooltip?.Replace("\n", "");
                activeSkillContainer.Add(skillInfo);
            }
            
            foreach (var listener in mob.listeners)
            {
                if (listener.type is not (MobListenerSO.ListenerType.Buff or MobListenerSO.ListenerType.Passive)) 
                    continue;
                
                var skillInfo = skillInfoTemplate.CloneTree();
                if (listener.type is MobListenerSO.ListenerType.Passive)
                {
                    skillInfo.Q<Label>("name").text = $"Lv{listener.level + 1} {listener.data?.name}";
                }
                else
                {
                    skillInfo.Q<Label>("name").text = $"{listener.data?.name}";
                }
                skillInfo.Q<Label>("info").text = listener.Tooltip?.Replace("\n", "");
                
                if (listener.type is MobListenerSO.ListenerType.Passive 
                    && (listener as Buff.Buff) != null
                    && (listener as Buff.Buff).source == mob)
                {
                    passiveSkillContainer.Add(skillInfo);
                }
                else
                {
                    effectContainer.Add(skillInfo);
                }
            }
        }

        public void Hide()
        {
            masterElem.visible = false;
        }
    }
}