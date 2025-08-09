using System.Collections;
using System.Collections.Generic;
using Backend.Map;
using DocumentFormat.OpenXml.Bibliography;
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
        public abstract List<Databackend.GridBFSKeys> ProposeMovementGrids(IGridCollider origin, Databackend.GridBFSKeys fromKey);
        public abstract float ComputeDistance(Vector3Int from, Vector3Int to);
        
        public override IEnumerator OnPerform(RuntimeAction<MovementTarget> ract, MobData mob, MovementTarget movementTarget)
        {
            // TODO: Move MoveToCoroutine to here and apply path.
            yield return new JumpIn(mob.MoveToCoroutine(movementTarget.destinationGrid, movementTarget.path));
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

        public List<Databackend.GridBFSKeys> ProposeMovementGrids(IGridCollider origin, Databackend.GridBFSKeys fromKey)
            => movementData.ProposeMovementGrids(origin, fromKey);

        public float ComputeDistance(Vector3Int from, Vector3Int to)
            => movementData.ComputeDistance(from, to);
    }

    public class WalkMovementSO : MovementSO
    {
        bool IsPassable(Vector3Int position, IGridCollider body, out GridData grid)
        {
            grid = Globals.backend.GetMap(position, false);
            if (grid.passable)
            {
                return Globals.backend.CanPositionPlaceMob(position, body);
            }

            return false;
        }

        bool IsSupported(Vector3Int position, IGridCollider body, out GridData supportingGrid)
        {
            supportingGrid = Globals.backend.GetMap(position, false);
            if (supportingGrid.standable)
            {
                return true;
            }
            
            supportingGrid = Globals.backend.GetMap(position + Vector3Int.down, false);
            return supportingGrid.standable;
        }
        
        static Vector3Int[] walkPositions =
        {
            Vector3Int.forward,
            Vector3Int.back,
            Vector3Int.left,
            Vector3Int.right
        };
        
        public override List<Databackend.GridBFSKeys> ProposeMovementGrids(
            IGridCollider origin, Databackend.GridBFSKeys fromKey)
        {
            List<Databackend.GridBFSKeys> keys = new();
            
            Vector3Int gridPos = Databackend.BackendToGridPos(origin.Position);

            int canJumpHeight = 0;
            for (int jumpD = 1; jumpD <= 2; jumpD++)
            {
                // Passable + Standable almost always is a platform
                // Platforms blocks jumping
                if (IsPassable(gridPos + Vector3Int.up * jumpD, origin, out var headGrid) && !headGrid.standable)
                {
                    canJumpHeight++;
                }
                else
                {
                    break;
                }
            }
            
            foreach (var wp in walkPositions)
            {
                var canWalkTo = IsPassable(gridPos + wp, origin, out var toGrid);
                var wontFall = IsSupported(gridPos + wp, origin, out var supportGrid);

                if (canWalkTo && wontFall)
                {
                    // TODO: Handle distance
                    keys.Add(new Databackend.GridBFSKeys(gridPos + wp, fromKey.distance + 1));
                }

                // Fall
                if (canWalkTo && (!wontFall))
                {
                    for (int fallD = 1; fallD <= 3; fallD++)
                    {
                        var wfp = gridPos + wp + Vector3Int.down * fallD;
                        
                        var canFallTo = IsPassable(wfp, origin, out var fallToGrid);
                        var fallSupported = IsSupported(wfp, origin, out var supportFallGrid);

                        if (canFallTo && fallSupported)
                        {
                            keys.Add(new Databackend.GridBFSKeys(wfp, fromKey.distance + 1));
                        }
                        else if (!canFallTo)
                        {
                            break;
                        }
                    }
                }
                
                // Jump
                for (int jumpD = 1; jumpD <= canJumpHeight; jumpD++)
                {
                    var jwp = gridPos + Vector3Int.up * jumpD + wp;
                    
                    var canJumpTo = IsPassable(jwp, origin, out var jumpToGrid);
                    var jumpSupported = IsSupported(jwp, origin, out var supportJumpGrid);

                    if (canJumpTo && jumpSupported)
                    {
                        keys.Add(new Databackend.GridBFSKeys(jwp, fromKey.distance + 1));
                    }
                }
            }

            return keys;
        }

        public override float ComputeDistance(Vector3Int from, Vector3Int to)
        {
            return 1;
            // throw new System.NotImplementedException();
        }
    }
}