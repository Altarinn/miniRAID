using UnityEngine;
using System.Collections;

namespace miniRAID.UI.TargetRequester
{
    public class ConfirmRequester : TargetRequesterBase
    {
        GridOverlay.Types type;

        public void InitRequester(GridOverlay.Types type)
        {
            this.type = type;
        }

        public override RequestStage Next(Vector2Int coord, bool notFirst = true)
        {
            if (currentStageCompleted >= 1)
            {
                Decided(coord);
                return null;
            }

            RequestStage stage = new RequestStage();
            stage.type = RequestType.Target;

            stage.map.Add(coord, this.type);

            return stage;
        }

        void Decided(Vector2Int coord)
        {
            Finish(new Spells.SpellTarget(coord));
        }
    }
}
