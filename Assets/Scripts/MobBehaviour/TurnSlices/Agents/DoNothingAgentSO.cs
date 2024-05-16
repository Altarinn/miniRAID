using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using miniRAID.TurnSchedule;
using UnityEngine;

namespace miniRAID.Agents
{
    [CreateAssetMenu(menuName = "Agents/NullAgent")]
    public class DoNothingAgentSO : MobTurnSliceBaseSO
    {
        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            yield return new JumpIn(((MobTurnSlice)slice).mob.SetActive(false));
        }
    }
}