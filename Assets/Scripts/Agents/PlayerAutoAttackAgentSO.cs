using System.Collections;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.Agents
{
    [CreateAssetMenu(menuName = "Agents/PlayerAutoAttackAgent")]
    public class PlayerAutoAttackAgentSO : MobListenerSO
    {
        public PlayerAutoAttackAgentSO() { type = ListenerType.Agent; }
        
        public override MobListener Wrap(MobData parent)
        {
            return new PlayerAutoAttackAgentBase(parent, this);
        }
    }

    public class PlayerAutoAttackAgentBase : MobListener
    {
        public PlayerAutoAttackAgentSO autoAttackData => (PlayerAutoAttackAgentSO)data;
        
        public PlayerAutoAttackAgentBase(MobData parent, PlayerAutoAttackAgentSO data) : base(parent, data)
        {
        }
        
        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnAutoAttackAgentWakeUp += OnAgentWakeUp;
            mob.OnMobSelectedInUI += MobOnOnMobSelectedInUI;
            mob.OnMobDeselectedInUI += MobOnOnMobDeselectedInUI;
        }

        public override void OnRemove(MobData mob)
        {
            base.OnRemove(mob);

            mob.OnAutoAttackAgentWakeUp -= OnAgentWakeUp;
            mob.OnMobSelectedInUI -= MobOnOnMobSelectedInUI;
            mob.OnMobDeselectedInUI -= MobOnOnMobDeselectedInUI;
        }

        private void MobOnOnMobSelectedInUI(MobData mob)
        {
            var sTarget = GetTarget(mob);
            if (sTarget != null)
            {
                AddIndicator(new SimpleSpriteIndicator(
                    null, 
                    Globals.backend.GridToWorldPos(GetTarget(mob).targetPos[0]) + Vector3.back * 5.0f));
            }
        }
        
        private void MobOnOnMobDeselectedInUI(MobData mob)
        {
            RemoveAllIndicators();
        }

        protected IEnumerator OnAgentWakeUp(MobData mob)
        {
            // Try to cast a regular attack, do it 10 times so we ensure all possible APs have been consumed
            // TODO: Do a proper termination check lmao
            for (int i = 0; i < 1; i++)
            {
                yield return new JumpIn(TryCastRegularAttack(mob));
            }
            
            // End mob's turn
            yield return new JumpIn(mob.SetActive(false));
        }

        protected IEnumerator TryCastRegularAttack(MobData mob)
        {
            // Return if mob has already performed an regular attack in this turn
            if(mob.IsInGCD(GCDGroup.RegularAttack))
                yield break;
            
            // Return if mob has no main weapon
            if(mob.mainWeapon == null)
                yield break;
            
            // Obtain a target
            SpellTarget target = mob.mainWeapon.QueryTarget(mob);
            
            // Return if mob has no target
            if(target == null)
                yield break;

            RuntimeAction ract = mob.mainWeapon.GetRegularAttackSpell();
            
            // Return if mob has no regular attack spell
            if(ract == null)
                yield break;

            // Cast the action
            // yield return new JumpIn(mob.DoActionWithDefaultCosts(ract, target));
            
            // Cast the action freely
            yield return new JumpIn(mob.DoAction(ract, target));
        }
        
        public SpellTarget GetTarget(MobData mob)
        {
            return mob.mainWeapon.QueryTarget(mob);
        }
    }
}