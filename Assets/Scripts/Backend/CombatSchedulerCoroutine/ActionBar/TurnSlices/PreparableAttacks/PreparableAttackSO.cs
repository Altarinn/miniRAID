using miniRAID.Spells;

namespace miniRAID.TurnSchedule
{
    public class PreparableAttackSO<TSpellTarget> : CustomIconScriptableObject where TSpellTarget : SpellTarget
    {
        public ActionDataSO<TSpellTarget> action;
        
        // Target picking
        // Turn scheduling (adding attack turns)
        // Condition to be chosen by BasicAttackPreparationTurnSlice
    }
}