using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace miniRAID.Agents
{
    public class SlimeKingAgent : MonoBehaviourAgentComponent
    {
        [System.Serializable]
        [InlineProperty(LabelWidth = 30)]
        public struct TurnSchedule
        {
            [HorizontalGroup]
            [LabelText("RAtk")]
            public bool doRegularAttack;

            [HorizontalGroup]
            [HideLabel]
            public ActionSOEntry action;
        }

        public List<TurnSchedule> schedule;

        private void Awake()
        {
            MobData mob = GetComponent<MobRenderer>().data;
        }

        public override IEnumerator Act(MobData mob, int turn)
        {
            TurnSchedule todo = schedule[(turn - 1) % schedule.Count];

            RuntimeAction ract = null;
            if (todo.action.data != null)
            {
                ract = mob.GetAction(todo.action.data);
                if (ract == null)
                {
                    Debug.LogWarning($"Specified action {todo.action.data.name} in schedule has not been added to the mob {mob.nickname}. Adding it automatically.");
                    ract = mob.AddAction(todo.action);

                    if (ract == null)
                    {
                        Debug.LogError($"Cannot add {todo.action.data.name} to mob {mob.nickname}. Action skipped.");
                    }
                }

                if (ract.level != todo.action.level)
                {
                    Debug.LogError($"Action {todo.action.data.name}: level in schedule does not match the level of the action in the mob.");
                }
            }
            
            if(ract != null && agent.currentTarget != null)
            {
                yield return new JumpIn(mob.DoActionWithDefaultCosts(
                    ract,
                    new Spells.SpellTarget(agent.currentTarget.Position)
                ));
            }

            if (todo.doRegularAttack)
            {
                yield return new JumpIn(agent.Act(mob));
            }

            yield return new JumpIn(mob.SetActive(false));
        }

        public override string GetIncomingString(MobData mob, int turn)
        {
            if(schedule.Count == 0)
            {
                return "Regular Attack";
            }

            int nextTurn = turn - 1;
            nextTurn = nextTurn % schedule.Count;

            string result = "";

            if(schedule[nextTurn].action.data != null) 
            { result += $"{schedule[nextTurn].action.data.ActionName}\n"; }

            if(schedule[nextTurn].doRegularAttack)
            { result += "Regular Attack\n"; }

            return result.TrimEnd('\r', '\n');
        }
    }
}
