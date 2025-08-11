using System.Collections;
using System.Collections.Generic;
using System.Linq;
using miniRAID;
using UnityEngine;

namespace miniRAID
{
    public class BlinkMovement : MovementSO
    {
        public IGridCollider moveableRange;
        
        public override List<Databackend.GridBFSKeys> ProposeMovementGrids(IGridCollider origin, Databackend.GridBFSKeys fromKey)
        {
            var gridPos = Databackend.BackendToGridPos(origin.Position);
            
            return moveableRange.Where(
                x =>
                {
                    if (x == Vector3Int.zero) return false;
                    var newGridPos = x + gridPos;
                    return true;
                }
            ).Select(x => new Databackend.GridBFSKeys(x + gridPos, fromKey.distance + 1)).ToList();
        }

        public override float ComputeDistance(Vector3Int from, Vector3Int to)
        {
            return 0;
        }

        public override IEnumerator MovementStepAnimation(MobData mob, Vector3Int targetPos)
        {
            // Play no animation and blink to target position
            yield break;
        }

        public override bool CanEndTurnAt(Vector3Int at, IGridCollider collider)
        {
            Debug.LogWarning("Blink Movement is being set as main movement method of mob. It allows stay mid-air.");
            return true;
        }
    }
}