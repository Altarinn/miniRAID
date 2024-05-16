using miniRAID.TurnSchedule;

namespace miniRAID.MobBehaviour
{
    public class SimplePreparableActionSO : PreparableActionSO
    {
        public int actionDelay;
        public TurnSliceSO playerTurn; // Will be used for checking and insertion

        public MobActionTurnSliceSO sliceToInsert;

        public override void ModifySchedule(MobData mob, CombatSchedulerCoroutine coroutine)
        {
            var node = coroutine.turnSchedule.First;

            int delay = actionDelay;
            while (node != null && delay > 0)
            {
                if (node.Value.data == playerTurn)
                {
                    delay--;
                }
                // else if (node.Value.data is RecoveryTurnSliceSO)
                // {
                //     // We don't have enough player turns
                //     // Insert them before the recovery stage
                //     for(;delay > 0; delay--)
                //     {
                //         coroutine.InsertTurnSliceBefore(node, playerTurn.Wrap(new TurnSliceMetadata(slice)));
                //     }
                // }

                // Insert the action turn here
                if (delay <= 0)
                {
                    RuntimeAction ract = mob.GetAction(action.data)
                                           ?? mob.AddAction(action);

                    coroutine.InsertTurnSliceBefore(node, sliceToInsert.Wrap(
                        mob, ract, new TurnSliceMetadata(mob)));
                }

                node = node.Next;
            }
        }
    }
}