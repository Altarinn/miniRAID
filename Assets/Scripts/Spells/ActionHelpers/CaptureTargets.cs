using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.ActionHelpers
{
    public static class CaptureTargetsInGridShape
    {
        public static List<MobData> CaptureAllTargetsWithinRange(MobData src, UnitFilters filter, HashSet<Vector3Int> grids)
        {
            return Globals.backend.GetAllMobs()
                .Where(m => filter.Check(src, m))
                .Where(m => grids.Contains(m.Position)) // IEnumerable<MobData>; TODO: Use m.gridBody instead of m.Position
                .ToList();
        }
    }
}