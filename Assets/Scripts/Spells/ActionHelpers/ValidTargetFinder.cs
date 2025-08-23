using System.Collections.Generic;
using UnityEngine;
using miniRAID.Spells;

namespace miniRAID.ActionHelpers
{
    public static class ValidTargetFinder
    {
        /// <summary>
        /// Finds a valid SingleMobTarget by testing each grid position occupied by the target mob.
        /// Returns the first valid position where the source mob can target the target mob with the given action.
        /// </summary>
        /// <param name="sourceMob">The mob performing the action</param>
        /// <param name="targetMob">The mob to target</param>
        /// <param name="actionData">The action to validate targeting for</param>
        /// <returns>Valid SingleMobTarget if found, null otherwise</returns>
        public static SingleMobTarget FindValidSingleMobTarget(MobData sourceMob, MobData targetMob, ActionDataSO actionData)
        {
            if (sourceMob == null || targetMob == null || actionData == null)
                return null;

            // Test each grid position occupied by the target mob
            foreach (Vector3Int targetPos in targetMob.Collider)
            {
                SingleMobTarget candidateTarget = new SingleMobTarget(targetMob, targetPos);
                if (actionData.CheckWithAbstractTargets(sourceMob, candidateTarget))
                {
                    return candidateTarget; // Found a valid position
                }
            }

            return null; // No valid position found
        }

        /// <summary>
        /// Finds a valid SingleMobTarget using a RuntimeAction wrapper.
        /// </summary>
        /// <param name="sourceMob">The mob performing the action</param>
        /// <param name="targetMob">The mob to target</param>
        /// <param name="runtimeAction">The runtime action to validate targeting for</param>
        /// <returns>Valid SingleMobTarget if found, null otherwise</returns>
        public static SingleMobTarget FindValidSingleMobTarget(MobData sourceMob, MobData targetMob, RuntimeAction<SingleMobTarget> runtimeAction)
        {
            return FindValidSingleMobTarget(sourceMob, targetMob, runtimeAction?.data);
        }
    }
}