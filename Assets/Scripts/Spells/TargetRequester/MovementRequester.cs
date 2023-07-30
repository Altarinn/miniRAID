using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.UI.TargetRequester
{
    public class MovementRequester : TargetRequesterBase
    {
        public bool overrideMobMovement = false;
        public int moveRange = 3;
        public int extraRange = 0;
        public BaseMobDescriptorSO.MovementType moveType;

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
            
            Dictionary<Vector3Int, float> moveRange;
            if (overrideMobMovement)
            {
                moveRange = Databackend.GetSingleton().GetMoveableGrids(mob.Position, this.moveRange, extraRange, moveType);
            }
            else
            {
                moveRange = Databackend.GetSingleton().GetMoveableGrids(mob);
            }

            stage.type = RequestType.Ground;

            foreach (var gridPos in moveRange)
            {
                stage.map.Add(gridPos.Key, gridPos.Value >= mob.actionPoints ? GridOverlay.Types.MOVE : GridOverlay.Types.BUFF);
            }

            return stage;
        }

        void Decided()
        {
            Finish(new Spells.SpellTarget(choice));
        }
    }
}