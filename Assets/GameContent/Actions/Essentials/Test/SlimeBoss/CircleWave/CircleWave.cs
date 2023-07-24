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
        private SimpleMultiTurnAction multiTurnWrapper = new SimpleMultiTurnAction(4);
        
        public GridShape innerCircleShape;
        public GridShape middleRingShape;
        public GridShape outerRingShape;

        public override IEnumerator OnPerform(RuntimeAction ract, MobData mob,
            Spells.SpellTarget target)
        {
            yield return new JumpIn(multiTurnWrapper.Do(Action, mob, target, true));
        }

        private IEnumerator Action(int turn, SimpleMultiTurnActionProxy dummy, MobData src, SpellTarget target, object customdata)
        {
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
                    dummy.RemoveAllIndicators();
                    
                    // Show middle ring warning
                    middleRingShape.position = target.targetPos[0];
                    dummy.AddIndicator(new GridShapeIndicator(middleRingShape, GridOverlay.Types.INCOMING_ATTACK))
                        ?.Move(Vector3.forward * 10.0f);
                    break;
                case 3:
                    // Ring explodes
                    dummy.RemoveAllIndicators();
                    
                    // Show outer warning
                    outerRingShape.position = target.targetPos[0];
                    dummy.AddIndicator(new GridShapeIndicator(outerRingShape, GridOverlay.Types.INCOMING_ATTACK))
                        ?.Move(Vector3.forward * 10.0f);
                    break;
                case 4:
                    // Outer explodes
                    dummy.RemoveAllIndicators();
                    break;
            }
            
            yield break;
        }
    }
}
