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
        
        public GridCollider innerCircleShape;
        public GridCollider middleRingShape;
        
        private GridCollider outerRingShape;

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
                    innerCircleShape.Position = target.Target.Position;
                    dummy.renderer = new GridColliderIndicator(innerCircleShape, GridOverlay.Types.INCOMING_ATTACK)
                        .Move(Vector3.forward * 10.0f);
                    break;

                case 2:
                    // Inner explodes
                    innerCircleShape.Position = target.Target.Position;
                    captured = CaptureTargetsInCollider.CaptureAllTargetsWithinRange(
                            src, filters, innerCircleShape, null)
                        .ToList();
                    yield return new JumpIn(
                        MobListHelpers.WaitForAllMobs(captured, m => damage.Do(ract, src, m))
                    );

                    dummy.renderer.Destroy();

                    // Show middle ring warning
                    middleRingShape.Position = target.Target.Position;
                    dummy.renderer = new GridColliderIndicator(middleRingShape, GridOverlay.Types.INCOMING_ATTACK)
                        ?.Move(Vector3.forward * 10.0f);
                    break;

                case 3:
                    // Ring explodes
                    middleRingShape.Position = target.Target.Position;
                    captured = CaptureTargetsInCollider.CaptureAllTargetsWithinRange(
                            src, filters, middleRingShape, null)
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
