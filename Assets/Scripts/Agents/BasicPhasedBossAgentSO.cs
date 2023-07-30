using System.Collections;
using System.Linq;
using miniRAID.Spells;
using miniRAID.UI.TargetRequester;
using Sirenix.OdinInspector;
using UnityEngine;

namespace miniRAID.Agents
{
    [CreateAssetMenu(menuName = "Agents/BasicPhasedBossAgent")]
    public class BasicPhasedBossAgentSO : AggroAgentBaseSO
    {
        [System.Serializable]
        // [ColoredBox("#ffcc66")]
        public struct PhaseTransitionCondition
        {
            [LabelText("Health % <=")] public float HealthPercentageLessThan;
            [LabelText("Health % >=")] public float HealthPercentageMoreThan;

            public bool canReturnTo;
        }

        [System.Serializable]
        public struct PhaseScheduleEntry
        {
            [LabelText("Regular Attack")]
            public bool doRegularAttack;

            public ActionSOEntry[] actions;
        }
        
        [System.Serializable]
        public class PhaseDefinition
        {
            public PhaseTransitionCondition condition;
            public bool loop;
            public PhaseScheduleEntry[] phaseSchedule;

            public int Turns => phaseSchedule.Length;
        }

        public PhaseDefinition[] phases;

        public static bool CheckPhaseCondition(PhaseTransitionCondition cond, MobData mob)
        {
            return 
                   ((mob.health / (float)mob.maxHealth) <= cond.HealthPercentageLessThan) 
                && ((mob.health / (float)mob.maxHealth) >= cond.HealthPercentageMoreThan);
        }

        public override MobListener Wrap(MobData parent)
        {
            return new BasicPhasedBossAgent(parent, this);
        }
    }

    public class BasicPhasedBossAgent : AggroAgentBase
    {
        private BasicPhasedBossAgentSO bpbaData => (BasicPhasedBossAgentSO)data;
        
        public BasicPhasedBossAgent(MobData parent, AggroAgentBaseSO data) : base(parent, data)
        {
            this.data = data;
            visitedPhase = new bool[bpbaData.phases.Length];
        }

        public BasicPhasedBossAgentSO.PhaseDefinition currentPhase;
        public int turnInPhase;
        public bool[] visitedPhase;

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            GoToPhase(0);
        }

        public struct ProgressData
        {
            public int currentPhaseIdx;
            public int turnInPhase;
            public bool[] visitedPhase;
        }
        
        public void CopyFrom(ProgressData other)
        {
            currentPhase = bpbaData.phases[other.currentPhaseIdx];
            turnInPhase = other.turnInPhase;
            visitedPhase = other.visitedPhase;
        }

        public ProgressData GetProgress()
        {
            return new ProgressData()
            {
                currentPhaseIdx = bpbaData.phases.ToList().FindIndex(p => p == currentPhase),
                turnInPhase = turnInPhase,
                visitedPhase = (bool[]) visitedPhase.Clone()
            };
        }

        protected override IEnumerator OnAgentWakeUp(MobData mob)
        {
            yield return new JumpIn(HandlePhaseTransition(mob));
            
            yield return new JumpIn(PerformActions(mob));

            yield return new JumpIn(base.OnAgentWakeUp(mob));
            
            turnInPhase += 1;
        }

        protected IEnumerator HandlePhaseTransition(MobData mob)
        {
            int phaseIdxToGo = -1;
            
            // Check if current phase is finished and it is not a loop
            if (turnInPhase >= currentPhase.Turns && !currentPhase.loop)
            {
                // Find the next phase
                for (int i = bpbaData.phases.Length - 1; i >= 0; i--)
                {
                    if (bpbaData.phases[i] == currentPhase && i < bpbaData.phases.Length - 1)
                    {
                        phaseIdxToGo = i + 1;
                        break;
                    }
                }
            }
            
            for (int i = bpbaData.phases.Length - 1; i >= 0; i--)
            {
                if (!visitedPhase[i] && 
                    BasicPhasedBossAgentSO.CheckPhaseCondition(bpbaData.phases[i].condition, mob))
                {
                    phaseIdxToGo = i;
                    break;
                }
            }

            if (phaseIdxToGo >= 0 && bpbaData.phases[phaseIdxToGo] != currentPhase)
            {
                GoToPhase(phaseIdxToGo);
                // TODO: Phase transition animation?
            }

            yield break;
        }

        protected void GoToPhase(int i)
        {
            Debug.Log($"[BasicPhasedBossAgent]: Mob {parentMob.nickname} is going to Phase #{i+1}!");
            
            var phase = bpbaData.phases[i];
            if (!phase.condition.canReturnTo)
            {
                visitedPhase[i] = true;
            }

            turnInPhase = 0;
            currentPhase = phase;
        }

        protected IEnumerator PerformActions(MobData mob)
        {
            BasicPhasedBossAgentSO.PhaseScheduleEntry todo =
                currentPhase.phaseSchedule[turnInPhase % currentPhase.Turns];

            foreach (var asoe in todo.actions)
            {
                if (mob == null || !mob.isControllable)
                {
                    break;
                }
                
                RuntimeAction ract = null;
                if (asoe.data != null)
                {
                    ract = mob.GetAction(asoe.data);
                    if (ract == null)
                    {
                        Debug.LogWarning($"Specified action {asoe.data.name} in schedule has not been added to the mob {mob.nickname}. Adding it automatically.");
                        ract = mob.AddAction(asoe);

                        if (ract == null)
                        {
                            Debug.LogError($"Cannot add {asoe.data.name} to mob {mob.nickname}. Action skipped.");
                        }
                    }

                    if (ract.level != asoe.level)
                    {
                        Debug.LogError($"Action {asoe.data.name}: level in schedule does not match the level of the action in the mob.");
                    }
                }
            
                if(ract != null && currentTarget != null)
                {
                    Spells.SpellTarget sTarget;
                
                    if (ract.data?.Requester?.GetType().IsAssignableFrom(typeof(ConfirmRequester)) ?? false)
                    {
                        sTarget = new SpellTarget(mob.Position);
                    }
                    else
                    {
                        sTarget = new Spells.SpellTarget(currentTarget.Position);
                    }
                
                    yield return new JumpIn(mob.DoActionWithDefaultCosts(
                        ract,
                        sTarget
                    ));
                }
            }
        }

        public override string GetIncomingString(MobData mob)
        {
            BasicPhasedBossAgentSO.PhaseScheduleEntry todo =
                currentPhase.phaseSchedule[turnInPhase % currentPhase.Turns];

            string result = "";

            foreach (var asoe in todo.actions)
            {
                if(asoe.data != null) 
                { result += $"{asoe.data.ActionName}\n"; }
            }

            if(todo.doRegularAttack)
            { result += "Regular Attack\n"; }

            return result.TrimEnd('\r', '\n');
        }

        public override string GetInformationString()
        {
            return $"Phase {bpbaData.phases.ToList().FindIndex(p => p == currentPhase) + 1}, Loop turn {turnInPhase}";
        }
    }
}