using miniRAID.Agents;
using miniRAID.Spells;

namespace miniRAID.TurnSchedule
{
    public class PreparableAttackSO : CustomIconScriptableObject
    {
        public ActionSOEntry action;
        public MobProxyAgentSO agent;

        // Target picking
        // Turn scheduling (adding attack turns)
        // Condition to be chosen by BasicAttackPreparationTurnSlice
    }
}