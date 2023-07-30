using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace miniRAID.UI.TargetRequester
{
    // TODO: Implement this
    public class FourDirectionalRequester : TargetRequesterBase
    {
        public GridShape shape;
        public GridOverlay.Types type;

        public override RequestStage Next(Vector3Int coord, bool notFirst = true)
        {
            // Empty
            RequestStage stage = new RequestStage();
            return stage;
        }

        public override void Request(MobData mob, RuntimeAction ract, OnRequestFinish onFinish, System.Action onCancel)
        {
            this.mob = mob;
            this.ract = ract;
            choice.Clear();

            this.onFinish = onFinish;
            this.onCancel = onCancel;

            ui.EnterState(this, true);
            ui.cursor.cursorShape = shape;
        }

        public override void Submit(InputValue input)
        {
            _Next(ui.cursor.position);
            Finish(new Spells.SpellTarget(choice));
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            UpdateCursor(ui.cursor.position);
        }

        public override void PointAtGrid(Vector3Int gridPos)
        {
            base.PointAtGrid(gridPos);

            UpdateCursor(gridPos);
        }

        void UpdateCursor(Vector3Int gridPos)
        {
            var dirc = Globals.backend.GetDominantDirection(mob.Position, gridPos);
            if (dirc != shape.direction)
            {
                shape.direction = dirc;
                ui.cursor.cursorShape = shape;
            }

            switch (dirc)
            {
                case GridShape.Direction.Up:
                    ui.cursor.position = mob.Position + new Vector3Int(0, 0, 1);
                    break;
                case GridShape.Direction.Left:
                    ui.cursor.position = mob.Position + new Vector3Int(-1, 0, 0);
                    break;
                case GridShape.Direction.Down:
                    ui.cursor.position = mob.Position + new Vector3Int(0, 0, -1);
                    break;
                case GridShape.Direction.Right:
                    ui.cursor.position = mob.Position + new Vector3Int(1, 0, 0);
                    break;
            }
        }
    }
}