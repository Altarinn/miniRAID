using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Backend.Map;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID
{
    public class MovementTarget : SpellTarget
    {
        public Vector3Int destinationGrid;
        public GridPath path;

        // TODO: Implement this? perhaps no need as this only used in requester
        public override bool Valid => Globals.backend.InMap(destinationGrid);
    }
    
    public abstract class MovementSO : ActionDataSO<MovementTarget>
    {
        public abstract List<Databackend.GridBFSKeys> ProposeMovementGrids(GridCollider origin, Databackend.GridBFSKeys fromKey);
        public abstract float ComputeDistance(Vector3Int from, Vector3Int to);
        public abstract bool CanEndTurnAt(Vector3Int at, GridCollider collider);

        public bool ignoreCostByDistance;
        public bool ignoreMovementFiltering;
        
        protected bool IsPassable(Vector3Int position, GridCollider body, out GridData grid)
        {
            grid = Globals.backend.GetMap(position, false);
            if (grid.passable)
            {
                return Globals.backend.CanPositionPlaceMob(position, body);
            }

            return false;
        }

        protected bool IsSupported(Vector3Int position, out GridData supportingGrid)
        {
            supportingGrid = Globals.backend.GetMap(position, false);
            if (supportingGrid.standable)
            {
                return true;
            }
            
            supportingGrid = Globals.backend.GetMap(position + Vector3Int.down, false);
            return supportingGrid.standable && IntrusionBits.GetFaceIntrusion(supportingGrid.intrusion, Vector3Int.up) == 0;
        }
        
        public IEnumerator MoveToCoroutine(MobData mob, Movement movement, Vector3Int targetPos, bool doCost = true)
        {
            // Check if targetPos is valid; If not, terminate the movement
            if (!Globals.backend.CanPositionPlaceMob(targetPos, mob.Collider))
            {
                yield return -1;
            }
            
            // TODO: implement path for field effects (move w.r.t. the path & tell backend that we reached a intermediate point)
            if (Globals.cc.animation && mob.mobRenderer != null)
                yield return new JumpIn(MovementStepAnimation(mob, targetPos));
            
            float distance = movement.ComputeDistance(Databackend.BackendToGridPos(mob.Position), targetPos);

            // Tell backend that we finished the movement

            // TODO: Move MoveRange / movedGrids / actedThisTurn etc. to Movement instance
            if (doCost)
            {
                mob.UseActionPoint(
                    Mathf.Max(0, distance - (mob.actedThisTurn ? 0 : (mob.MoveRange - mob.movedGrids))));

                if (!mob.actedThisTurn)
                {
                    mob.movedGrids = Mathf.Min(mob.MoveRange, mob.movedGrids + distance);
                }
            }

            yield return new JumpIn(mob.SetPosition(targetPos));
        }

        public virtual IEnumerator MovementStepAnimation(MobData mob, Vector3Int targetPos)
        {
            yield return new JumpIn(mob.mobRenderer.MoveTowards(targetPos));
        }
        
        public override IEnumerator OnPerform(RuntimeAction<MovementTarget> ract, MobData mob, MovementTarget movementTarget)
        {
            // TODO: Move MoveToCoroutine to here and apply path.
            foreach (var step in movementTarget.path.path)
            {
                yield return new JumpIn(MoveToCoroutine(mob, (Movement)ract, step, !ignoreCostByDistance));
            }
        }

        public override RuntimeAction<MovementTarget> LeveledWrap(MobData source, int level)
        {
            var movement = new Movement(source, this, level);
            movement.SetData(this);

            return movement;
        }
    }

    public class Movement : RuntimeAction<MovementTarget>
    {
        public MovementSO movementData => (MovementSO)data;
        
        public Movement(MobData source, ActionDataSO<MovementTarget> data, int level) : base(source, data, level)
        { }

        public virtual List<Databackend.GridBFSKeys> ProposeMovementGrids(GridCollider origin,
            Databackend.GridBFSKeys fromKey)
        {
            var result = movementData.ProposeMovementGrids(origin, fromKey);    
            if (!movementData.ignoreMovementFiltering && parentMob?.movement != null)
            {
                result = result.Where(x => parentMob.movement.CanEndTurnAt(x.position)).ToList();
            }

            return result;
        }

        public virtual float ComputeDistance(Vector3Int from, Vector3Int to)
            => movementData.ComputeDistance(from, to);

        public virtual bool CanEndTurnAt(Vector3Int position)
            => movementData.CanEndTurnAt(position, parentMob.Collider);
    }
}