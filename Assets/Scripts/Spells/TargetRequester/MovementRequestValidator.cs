using System.Collections;
using System.Collections.Generic;
using System.Linq;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.UI.TargetRequester
{
    public class MovementRequestValidator : TargetRequestValidatorBase<MovementTarget>
    {
        public bool overrideMobMovement = false;
        public int moveRange = 3;
        public int extraRange = 0;

        private Dictionary<Vector3Int, (Vector3Int prevGrid, float distance)> gridInfo;

        protected virtual int GridPathCost(GridData data)
        {
            if(data.solid || data.mob != null) { return 1; }
            return 0;
        }

        public override RequestStage Next(Vector3Int coord, bool notFirst = true)
        {
            if(currentStageCompleted >= 1)
            {
                Decided();
                return null;
            }

            RequestStage stage = new RequestStage();

            float resolvedMoveRange = mob.actedThisTurn ? 0 : mob.MoveRangeLeft;
            int resolvedExtraRange = Mathf.FloorToInt(mob.actionPoints);
            if (overrideMobMovement)
            {
                resolvedMoveRange = this.moveRange;
                resolvedExtraRange = extraRange;
            }

            Globals.backend.GenericMovementBFS(
                mob.Collider,
                (Movement)ract,
                x => false,
                Mathf.CeilToInt(resolvedMoveRange) + resolvedExtraRange,
                out gridInfo
            );
            
            stage.type = RequestType.Ground;

            foreach (var gridPos in gridInfo)
            {
                stage.map.Add(gridPos.Key, gridPos.Value.distance > resolvedMoveRange ? GridOverlay.Types.BUFF : GridOverlay.Types.MOVE);
            }

            return stage;
        }

        void Decided()
        {
            // Reconstruct path
            var result = new MovementTarget();
            result.path = Databackend.ReconstructPath(x => gridInfo[x].prevGrid, choice.First());
            result.destinationGrid = choice.First();
            
            Finish(result);
        }
    }
}