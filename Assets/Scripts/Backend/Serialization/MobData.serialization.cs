namespace miniRAID
{
    public partial class MobData
    {
        public void RestoreFromDeserialization()
        {
            // Insert every events here
            OnWakeup.RestoreListener();
            OnAgentWakeUp.RestoreListener();
            OnAutoAttackAgentWakeUp.RestoreListener();
            OnNextTurn.RestoreListener();
            OnRecoveryStage.RestoreListener();
            
            OnInitialized.RestoreListener();
            OnBaseStatCalculation.RestoreListener();
            OnStatCalculation.RestoreListener();
            OnActionStatCalculation.RestoreListener();
            OnStatCalculationFinish.RestoreListener();
            
            OnMobMoved.RestoreListener();
            OnQueryActions.RestoreListener();
            
            OnShowMobMenu.RestoreListener();
            
            OnDealDmg.RestoreListener();
            OnDamageDealt.RestoreListener();
            OnDealHeal.RestoreListener();
            OnHealDealt.RestoreListener();
            OnReceiveDamage.RestoreListener();
            OnBeforeDamageApplied.RestoreListener();
            OnDamageReceived.RestoreListener();
            OnReceiveHeal.RestoreListener();
            OnBeforeHealApplied.RestoreListener();
            OnHealReceived.RestoreListener();
            OnKill.RestoreListener();
            OnPreDeath.RestoreListener();
            OnRealDeath.RestoreListener();
            OnActionChosen.RestoreListener();
            OnActionPrecast.RestoreListener();
            OnActionPostcast.RestoreListener();
            OnModifyCost.RestoreListener();
            
            OnCheckCost.RestoreListener();
            OnApplyCost.RestoreListener();
            
            OnCostQueryDisplay.RestoreListener();
            
            OnMobSelectedInUI.RestoreListener();
            OnMobDeselectedInUI.RestoreListener();
            
            RecalculateStats();
        }
    }
}