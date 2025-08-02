using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using GameContent.Buffs.Test;
using miniRAID.ActionHelpers;
using miniRAID.Spells;

namespace miniRAID
{
    public class CircleWave : ActionDataSO<SingleMobTarget>, IMultiTurnActionBehaviour<SingleMobTarget>
    {
        private SimpleMultiTurnAction<SingleMobTarget> multiTurnWrapper = new SimpleMultiTurnAction<SingleMobTarget>(3);
        public UnitFilters filters;
        public SpellDamageHeal damage;
        
        public GridShape innerCircleShape;
        public GridShape middleRingShape;
        
        private GridShape outerRingShape;

        public override IEnumerator OnPerform(RuntimeAction<SingleMobTarget> ract, MobData mob,
            SingleMobTarget target)
        {
            yield return new JumpIn(multiTurnWrapper.Do(this, ract, mob, target, true));
        }

        public IEnumerator DoAction(
            RuntimeAction<SingleMobTarget> ract,
            int turn, SimpleMultiTurnActionProxy<SingleMobTarget> dummy, MobData src,
            SingleMobTarget target,
            object customData)
        {
            List<MobData> captured;
            switch (turn)
            {
                case 1:
                    // Show inner circ warning
                    innerCircleShape.position = target.Target.Position;
                    dummy.renderer = new GridShapeIndicator(innerCircleShape, GridOverlay.Types.INCOMING_ATTACK)
                        .Move(Vector3.forward * 10.0f);
                    break;

                case 2:
                    // Inner explodes
                    innerCircleShape.position = target.Target.Position;
                    captured = CaptureTargetsInGridShape.CaptureAllTargetsWithinRange(
                            src, filters, innerCircleShape.ApplyTransform())
                        .ToList();
                    yield return new JumpIn(
                        MobListHelpers.WaitForAllMobs(captured, m => damage.Do(ract, src, m))
                    );

                    dummy.renderer.Destroy();

                    // Show middle ring warning
                    middleRingShape.position = target.Target.Position;
                    dummy.renderer = new GridShapeIndicator(middleRingShape, GridOverlay.Types.INCOMING_ATTACK)
                        ?.Move(Vector3.forward * 10.0f);
                    break;

                case 3:
                    // Ring explodes
                    middleRingShape.position = target.Target.Position;
                    captured = CaptureTargetsInGridShape.CaptureAllTargetsWithinRange(
                            src, filters, middleRingShape.ApplyTransform())
                        .ToList();
                    yield return new JumpIn(
                        MobListHelpers.WaitForAllMobs(captured, m => damage.Do(ract, src, m))
                    );

                    dummy.renderer.Destroy();
                    dummy.renderer = null;
                    break;
                // Show outer warning
                //     innerCircleShape.position = target.targetPos[0];
                //     middleRingShape.position = target.targetPos[0];
                //     outerRingShape = GridShape.Negate(GridShape.Combine(
                //         innerCircleShape.ApplyTransform(), 
                //         middleRingShape.ApplyTransform()));
                //     outerRingShape.position = Vector3Int.zero;
                //
                //     dummy.AddIndicator(new GridShapeIndicator(
                //             outerRingShape,
                //             GridOverlay.Types.INCOMING_ATTACK))
                //         ?.Move(Vector3.forward * 10.0f);
                //     break;
                // case 4:
                //     // Outer explodes
                //     dummy.RemoveAllIndicators();
                //     break;
            }

            yield break;
        }
    }
}
