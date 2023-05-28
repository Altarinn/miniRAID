using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using miniRAID.Spells;

namespace miniRAID.Actions
{
    public class BasicChargeAction : Action
    {
        private void Awake()
        {
            ActionName = "Charge";

            Requester = new miniRAID.UI.TargetRequester.ConfirmRequester();
            (Requester as UI.TargetRequester.ConfirmRequester).InitRequester(GridOverlay.Types.BUFF);
        }

        protected override bool PreCorotine(MobData mob, SpellTarget target)
        {
            return true;
        }

        protected override IEnumerator Coroutine(MobData mob, SpellTarget target)
        {
            yield break;
        }

        public override bool DoCost(MobData mob, SpellTarget target)
        {
            mob.UseActionPoint(mob.actionPoints);
            return true;
        }
    }
}