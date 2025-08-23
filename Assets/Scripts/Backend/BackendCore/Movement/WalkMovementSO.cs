using System.Collections.Generic;
using UnityEngine;

namespace miniRAID
{
    public class WalkMovementSO : MovementSO
    {
        static Vector3Int[] walkPositions =
        {
            Vector3Int.forward,
            Vector3Int.back,
            Vector3Int.left,
            Vector3Int.right
        };

        public override List<Databackend.GridBFSKeys> ProposeMovementGrids(
            GridCollider origin, Databackend.GridBFSKeys fromKey)
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
                var wontFall = IsSupported(gridPos + wp, out var supportGrid);

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
                        var fallSupported = IsSupported(wfp, out var supportFallGrid);

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
                    var jumpSupported = IsSupported(jwp, out var supportJumpGrid);

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

        public override bool CanEndTurnAt(Vector3Int at, GridCollider collider)
        {
            return IsPassable(at, collider, out GridData g) && IsSupported(at, out GridData g2);
        }
    }
}