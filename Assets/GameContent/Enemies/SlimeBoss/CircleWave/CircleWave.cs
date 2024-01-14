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
    public class CircleWave : ActionDataSO
    {
        private SimpleMultiTurnAction multiTurnWrapper = new SimpleMultiTurnAction(3);
        public UnitFilters filters;
        public SpellDamageHeal damage;
        
        public GridShape innerCircleShape;
        public GridShape middleRingShape;
        
        private GridShape outerRingShape;

        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            yield return new JumpIn(multiTurnWrapper.Do(Action, mob, target, true));
            
            IEnumerator Action(int turn, SimpleMultiTurnActionProxy dummy, MobData src, SpellTarget target, object customdata)
            {
                List<MobData> captured;
                switch (turn)
                {
                    case 1:
                        // Show inner circ warning
                        innerCircleShape.position = target.targetPos[0];
                        dummy.AddIndicator(new GridShapeIndicator(innerCircleShape, GridOverlay.Types.INCOMING_ATTACK))
                            ?.Move(Vector3.forward * 10.0f);
                        break;
                    
                    case 2:
                        // Inner explodes
                        innerCircleShape.position = target.targetPos[0];
                        captured = CaptureTargetsInGridShape.CaptureAllTargetsWithinRange(
                                src, filters, innerCircleShape.ApplyTransform())
                            .ToList();
                        foreach (var m in captured)
                        {
                            yield return new JumpIn(damage.Do(ract, src, m));
                        }

                        dummy.RemoveAllIndicators();
                        
                        // Show middle ring warning
                        middleRingShape.position = target.targetPos[0];
                        dummy.AddIndicator(new GridShapeIndicator(middleRingShape, GridOverlay.Types.INCOMING_ATTACK))
                            ?.Move(Vector3.forward * 10.0f);
                        break;
                    
                    case 3:
                        // Ring explodes
                        middleRingShape.position = target.targetPos[0];
                        captured = CaptureTargetsInGridShape.CaptureAllTargetsWithinRange(
                                src, filters, middleRingShape.ApplyTransform())
                            .ToList();
                        foreach (var m in captured)
                        {
                            yield return new JumpIn(damage.Do(ract, src, m));
                        }
                        
                        dummy.RemoveAllIndicators();
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
}