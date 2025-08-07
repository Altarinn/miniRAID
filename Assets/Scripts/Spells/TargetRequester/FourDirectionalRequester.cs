using System.Collections;
using System.Linq;
using miniRAID.Spells;
using UnityEngine;
using UnityEngine.InputSystem;

namespace miniRAID.UI.TargetRequester
{
    // TODO: Implement this
    public class FourDirectionalRequester : TargetRequesterBase<FourDirectionalTarget>
    {
        public IGridCollider shape;
        public GridOverlay.Types type;

        public override RequestStage Next(Vector3Int coord, bool notFirst = true)
        {
            // Empty
            RequestStage stage = new RequestStage();
            return stage;
        }

        public override void Request(
            MobData mob, RuntimeAction<FourDirectionalTarget> ract, OnRequestFinish onFinish, System.Action onCancel)
        {
            this.mob = mob;
            this.ract = ract;
            choice.Clear();

            this.onFinish = onFinish;
            this.onCancel = onCancel;

            ui.EnterState(this, true);
            ui.cursor.ChangeCollider(shape);
        }

        public override void Submit(InputValue input)
        {
            _Next(ui.cursor.GridPos);
            
            var dirc = Globals.backend.GetDominantDirection(mob.GridPosition, choice.First());
            Finish(new FourDirectionalTarget(dirc));
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            UpdateCursor(ui.cursor.GridPos);
        }

        public override void PointAtGrid(Vector3Int gridPos)
        {
            base.PointAtGrid(gridPos);

            UpdateCursor(gridPos);
        }

        void UpdateCursor(Vector3Int gridPos)
        {
            var dirc = Globals.backend.GetDominantDirection(mob.GridPosition, gridPos);
            if (dirc != shape.Direction)
            {
                shape.Direction = dirc;
                ui.cursor.ChangeCollider(shape);
            }

            switch (dirc)
            {
                case Consts.Direction.Up:
                    ui.cursor.Position = mob.Position + new Vector3Int(0, 0, 1);
                    break;
                case Consts.Direction.Left:
                    ui.cursor.Position = mob.Position + new Vector3Int(-1, 0, 0);
                    break;
                case Consts.Direction.Down:
                    ui.cursor.Position = mob.Position + new Vector3Int(0, 0, -1);
                    break;
                case Consts.Direction.Right:
                    ui.cursor.Position = mob.Position + new Vector3Int(1, 0, 0);
                    break;
            }
        }
    }
}