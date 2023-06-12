using System.Collections;
using UnityEngine;

namespace miniRAID.UI.TargetRequester
{
    public class MovementRequester : TargetRequesterBase
    {
        public int range = 3;

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

            var moveRange = Databackend.GetSingleton().GetMoveableGrids(mob);

            stage.type = RequestType.Ground;

            foreach (var gridPos in moveRange)
            {
                stage.map.Add(gridPos, GridOverlay.Types.MOVE);
            }

            return stage;
        }

        void Decided()
        {
            Finish(new Spells.SpellTarget(choice));
        }
    }
}