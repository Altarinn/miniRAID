using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace miniRAID
{
	public partial class CombatSchedulerCoroutine : MonoBehaviour
    {
        #region Cut-In animations
        public IEnumerator UIPlayerPhase()
        {
            // notify UI
            Globals.debugMessage.Instance.Message($"玩家回合");

            yield break;
        }

        public IEnumerator UIEnemyPhase()
        {
            // notify UI
            Globals.debugMessage.Instance.Message($"敌方回合");

            yield break;
        }

        public IEnumerator UIAllyPhase()
        {
            // notify UI
            Globals.debugMessage.Instance.Message($"友方回合");

            yield break;
        }
        #endregion

        Mob chosenMob = null;

        System.Func<IEnumerator> actionToDo = null;

        public IEnumerator UIWaitPlayerInput()
        {
            // Refresh UI state
            Globals.ui.Instance.EnterState();

            bool shouldEnd = false;

            while(!shouldEnd)
            {
                if(actionToDo == null) { yield return null; }
                else
                {
                    yield return new JumpIn(actionToDo.Invoke());
                    shouldEnd = CheckPhaseEnd(awaitForActions);
                }
            }
        }

        // Uses lazy evaluation of IEnumerator.
        public void UIPickedAction(IEnumerator action, IEnumerator onActionFinished)
        {
            if(actionToDo != null)
            {
                Debug.LogError("CombatSchedulerCoroutine: UIPickedAction before previous action finished! Request ignored ...");
                return;
            }

            IEnumerator OnFinishWrapper() 
            {
                yield return new JumpIn(action);

                // Maybe some fading animation?
                yield return new JumpIn(onActionFinished);

                actionToDo = null;
            };

            actionToDo = OnFinishWrapper;
        }
    }
}
