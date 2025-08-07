using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.ActionHelpers
{
    public static class CaptureTargetsInCollider
    {
        public static List<MobData> CaptureAllTargetsWithinRange(MobData src, UnitFilters filter, IGridCollider range)
        {
            return Globals.backend.GetAllMobs()
                .Where(m => filter.Check(src, m))
                .Where(m => range.Overlaps(m.Collider)) 
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