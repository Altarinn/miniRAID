using UnityEngine;
using System.Collections;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID.UI.TargetRequester
{
    public class ConfirmRequestValidator : TargetRequestValidatorBase<SingleMobTarget>
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

        public override bool ValidateTargets(MobData mob, SingleMobTarget target)
        {
            return target.Target == mob;
        }

        void Decided(Vector3Int coord)
        {
            MobData mob = Essentials.MobAtGrid(coord);
            Finish(new Spells.SingleMobTarget(mob, coord));
        }
    }
}
