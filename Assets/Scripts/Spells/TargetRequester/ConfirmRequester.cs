using UnityEngine;
using System.Collections;
using miniRAID.Spells;

namespace miniRAID.UI.TargetRequester
{
    public class ConfirmRequester : TargetRequesterBase
    {
        GridOverlay.Types type;

        public void InitRequester(GridOverlay.Types type)
        {
            this.type = type;
        }

        public override RequestStage Next(Vector3Int coord, bool notFirst = true)
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

        public override bool CheckTargets(MobData mob, SpellTarget target)
        {
            return target.targetPos.Count == 1 && target.targetPos[0] == mob.Position;
        }

        void Decided(Vector3Int coord)
        {
            Finish(new Spells.SpellTarget(coord));
        }
    }
}
