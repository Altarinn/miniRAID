using System.Collections;
using System.Collections.Generic;
using miniRAID.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace miniRAID.Weapon
{
    [CreateAssetMenu(menuName = "Weapon/Bow")]
    public class BowSO : WeaponSO
    {
        public BowSO() : base()
        {
            wpType = WeaponType.Bow;
        }

        public ActionSOEntry aimedAttack;

        public override MobListener Wrap(MobData parent)
        {
            return new Bow(parent, this);
        }
    }

    public class Bow : Weapon
    {
        public BowSO bowData => (BowSO)data;

        bool moved = false;

        public Bow(MobData parent, BowSO data) : base(parent, data) { this.data = data; }
        RuntimeAction RaimedAttack;

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            RaimedAttack = mob.AddAction(bowData.aimedAttack);

            mob.OnNextTurn += OnNextTurn;
            mob.OnMobMoved += MobOnOnMobMoved;
        }

        public override void OnRemove(MobData mob)
        {
            // TODO: Remove action?
            mob.OnNextTurn -= OnNextTurn;
            mob.OnMobMoved -= MobOnOnMobMoved;
        }

        private void MobOnOnMobMoved(MobData mob, Vector3Int from)
        {
            moved = true;
        }

        private void OnNextTurn(MobData mob)
        {
            moved = false;
        }

        protected override void OnQueryActions(MobData mob, HashSet<RuntimeAction> actions)
        {
            base.OnQueryActions(mob, actions);
            if(moved == true)
            {
                actions.RemoveWhere(x => x.data == bowData.aimedAttack.data);
            }
        }

        public override string GetInformationString()
        {
            if(moved == true)
            {
                return $"瞄准：否";
            }
            return $"瞄准：是";
        }

        public override void ShowInUI(EquipmentController ui)
        {
            base.ShowInUI(ui);

            if (RaimedAttack != null)
            {
                var skillInfo = ui.specialAttackTemplate.CloneTree();
                RaimedAttack.ShowInUI(skillInfo.Q("skillContainer"));
                
                skillInfo.Q<Label>("specialTitle").text = GetWeaponSpecialAttackTitle();
                
                string specialAttackTooltip = GetWeaponSpecialAttackTooltip();
                if (specialAttackTooltip != null)
                {
                    var specialLabel = skillInfo.Query("specialDescription").Children<Label>().First();
                    specialLabel.text = specialAttackTooltip;
                }
                
                ui.container.Add(skillInfo);
            }
        }
    }
}
