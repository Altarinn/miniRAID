using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using miniRAID.Spells;

namespace miniRAID.Actions
{
    [CreateAssetMenu(menuName = "BasicActions/Move")]
    public class Move : Action
    {
        private void Awake()
        {
            Requester = new miniRAID.UI.TargetRequester.MovementRequester();
        }

        protected override bool PreCorotine(Mob mob, SpellTarget target)
        {
            var grid = Databackend.GetSingleton().getMap(target.targetPos[0].x, target.targetPos[0].y);

            // Move
            // TODO: can move
            if (grid.mob == null && grid.solid == false)
            {
                return true;
            }

            return false;
        }

        protected override IEnumerator Coroutine(Mob mob, SpellTarget target)
        {
            return mob.MoveToCorotine(target.targetPos[0], null, null);
        }
    }
}