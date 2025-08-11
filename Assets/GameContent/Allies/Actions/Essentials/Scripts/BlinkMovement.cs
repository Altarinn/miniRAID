using System.Collections.Generic;
using miniRAID;
using UnityEngine;

namespace GameContent.Allies.Actions.Essentials.Scripts
{
    public class BlinkMovement : MovementSO
    {
        public int distance = 4;
        
        public override List<Databackend.GridBFSKeys> ProposeMovementGrids(IGridCollider origin, Databackend.GridBFSKeys fromKey)
        {
            throw new System.NotImplementedException();
        }

        public override float ComputeDistance(Vector3Int from, Vector3Int to)
        {
            throw new System.NotImplementedException();
        }
    }
}