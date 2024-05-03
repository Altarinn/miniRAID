using System.Collections;
using miniRAID.Spells;

namespace miniRAID.TurnSchedule
{
    public class BasicAttackPreparationTurnSliceSO<TSpellTarget> : TurnSliceSO where TSpellTarget : SpellTarget
    {
        [System.Serializable]
        public struct AttackPatterns
        {
            public PreparableAttackSO<TSpellTarget> attack;
            public int priority;
            public float weight;
        }
        
        public override IEnumerator Turn(CombatSchedulerCoroutine coroutine)
        {
            // TODO: Choose random attack in all patterns
            throw new System.NotImplementedException();
        }
    }
}