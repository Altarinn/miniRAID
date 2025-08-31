using System.Collections;
using miniRAID.TurnSchedule;
using UnityEngine;

namespace miniRAID.MobBehaviour.TurnSlices
{
    public abstract class ClaimingPreparableTurnSliceSO : MobTurnSliceBaseSO
    {
        [SerializeField] private AbstractTurnSliceSO turnSeperator;
        
        public bool TryClaimNextTurn(PreparableActionTurnSliceSO targetSlice, MobData mob, RuntimeAction action)
        {
            var node = Globals.combatMgr.Instance.turnSchedule.First;

            bool found = false;
            
            while (node != null)
            {
                // Terminate until end of turn
                if (found && node.Value.data == turnSeperator)
                {
                    break;
                }
                
                if (node.Value.data == targetSlice && (node.Value is DummyTurnSlice))
                {
                    found = true;
                    var newSlice = targetSlice.Wrap(mob, action, new TurnSliceMetadata(mob));
                    Globals.combatMgr.Instance.turnSchedule.Claim(node, newSlice);
                }
                
                node = node.Next;
            }
            
            return found;
        }
    }
}