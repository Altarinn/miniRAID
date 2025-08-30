namespace miniRAID
{
    public partial class CombatTracker
    {
        public void RestoreFromSerialization()
        {
            combatLog = new LoggerWithUI("miniRAID.combat.log", false);
            this.SetUI(Globals.combatTracker.ui);
        }
    }
}