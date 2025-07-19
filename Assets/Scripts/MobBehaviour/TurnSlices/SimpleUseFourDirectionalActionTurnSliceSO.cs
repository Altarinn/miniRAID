using System.Collections;
using System.Linq;
using miniRAID.Agents;
using miniRAID.Backend;
using miniRAID.Spells;
using miniRAID.TurnSchedule;

namespace miniRAID.MobBehaviour.TurnSlices
{
    public class SimpleUseFourDirectionalActionTurnSliceSO : PreparableActionTurnSliceSO
    {
        // TODO: Lock or unlock
        public bool UpdateTargetAfterInitialized = false;

        public override MobActionTurnSlice Wrap(MobData mob, RuntimeAction action, TurnSliceMetadata metadata)
        {
            return new SimpleUseFourDirectionalActionTurnSlice(mob, action, this, metadata);
        }

        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SimpleUseFourDirectionalActionTurnSlice : PreparableActionTurnSlice, IRenderableState
    {
        private FourDirectionalTarget target;
        private GridShape indicatorShape;
        
        public SimpleUseFourDirectionalActionTurnSlice(
            MobData mob, RuntimeAction action, AbstractTurnSliceSO data, TurnSliceMetadata metadata)
            : base(mob, action, data, metadata)
        {
            target = GetTarget(mob, mob.FindListener<TargetIndicator>());

            indicatorShape = new GridShape(((RuntimeAction<FourDirectionalTarget>)action).Shape);
            indicatorShape.position = mob.Position;
            indicatorShape.direction = target.Target;
            
            ConstructRenderer();
        }
        
        private FourDirectionalTarget GetTarget(MobData self, TargetIndicator targetIndicator)
        {
            if (targetIndicator == null || targetIndicator.CurrentTarget == null)
            {
                Globals.ui.Instance.combatView.debugText.text = $"{mob.nickname}: {action.ActionName} -> NO Target";
                return new FourDirectionalTarget(Consts.Direction.Up);
            }
            
            Globals.ui.Instance.combatView.debugText.text = $"{mob.nickname}: {action.ActionName} -> {(((SimpleUseFourDirectionalActionTurnSliceSO)data).UpdateTargetAfterInitialized ? "" : "(LOCK) ")}{targetIndicator.CurrentTarget.nickname}";
            
            return new FourDirectionalTarget(
                Globals.backend.GetDominantDirection(self.Position, targetIndicator.CurrentTarget.Position));
        }

        protected override IEnumerator OnGlobalPostAction(MobData source, RuntimeAction action, SpellTarget target)
        {
            if (!((SimpleUseFourDirectionalActionTurnSliceSO)data).UpdateTargetAfterInitialized)
            {
                yield break;
            }

            if (action == this.action)
            {
                yield break;
            }

            if (mob.isDead)
            { Mute(); }

            this.target = GetTarget(mob, mob.FindListener<AggroCollector>());
            
            // TODO: Find better solution?
            indicatorShape.direction = this.target.Target;
            UpdateRenderer();
        }

        public override IEnumerator Turn()
        {
            if (mob.isDead)
            {
                yield break;
            }
            
            indicatorShape = null;
            UpdateRenderer();
            
            RuntimeAction<FourDirectionalTarget> act = (RuntimeAction<FourDirectionalTarget>)action;
            yield return new JumpIn(mob.DoActionWithDefaultCosts(act, target));
        }

        public void ConstructRenderer()
        {
            if (indicatorShape != null)
            {
                renderer = new GridShapeIndicator(
                    indicatorShape, GridOverlay.Types.INCOMING_ATTACK);
            }
            UpdateRenderer();
        }

        public void UpdateRenderer()
        {
            (renderer as GridShapeIndicator)?.Update(indicatorShape);
        }
    }
}