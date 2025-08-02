using System;
using System.Collections;
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

    public static class MobListHelpers
    {
        public static IEnumerator WaitForAllMobs(IEnumerable<MobData> mobs, Func<MobData, IEnumerator> getCoroutine)
        {
            CoroutineFence fence = new();
            foreach (MobData mob in mobs)
            {
                Globals.JumpInParallel(getCoroutine.Invoke(mob), fence);
            }

            while (fence.IsNotFinished()) yield return null;
        }
    }
}