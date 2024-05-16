using System.Collections;
using miniRAID.Spells;

namespace miniRAID.TurnSchedule
{
    public class BasicAttackPreparationTurnSliceSO : TurnSliceSO
    {
        [System.Serializable]
        public struct AttackPatterns
        {
            public PreparableAttackSO attack;
            public int priority;
            public float weight;
        }
        
        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            // TODO: Choose random attack in all patterns
            throw new System.NotImplementedException();
        }
    }
}