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
            public ActionDataSO action;
        }

        public List<TurnSchedule> schedule;

        private void Awake()
        {
            Mob mob = GetComponent<Mob>();
        }

        public override IEnumerator Act(Mob mob, int turn)
        {
            TurnSchedule todo = schedule[(turn - 1) % schedule.Count];

            RuntimeAction ract = null;
            if (todo.action != null)
            {
                ract = mob.GetAction(todo.action);
                if (ract == null)
                {
                    Debug.LogWarning($"Specified action {todo.action.name} in schedule has not been added to the mob {mob.name}. Adding it automatically.");
                    ract = mob.AddAction(todo.action);

                    if (ract == null)
                    {
                        Debug.LogError($"Cannot add {todo.action.name} to mob {mob.name}. Action skipped.");
                    }
                }
            }
            
            if(ract != null && agent.currentTarget != null)
            {
                yield return new JumpIn(mob.DoActionWithDefaultCosts(
                    ract,
                    new Spells.SpellTarget(agent.currentTarget.data.Position)
                ));
            }

            if (todo.doRegularAttack)
            {
                yield return new JumpIn(agent.Act(mob));
            }

            yield return new JumpIn(mob.SetActive(false));
        }

        public override string GetIncomingString(Mob mob, int turn)
        {
            if(schedule.Count == 0)
            {
                return "Regular Attack";
            }

            int nextTurn = turn - 1;
            nextTurn = nextTurn % schedule.Count;

            string result = "";

            if(schedule[nextTurn].action != null) 
            { result += $"{schedule[nextTurn].action.ActionName}\n"; }

            if(schedule[nextTurn].doRegularAttack)
            { result += "Regular Attack\n"; }

            return result.TrimEnd('\r', '\n');
        }
    }
}
