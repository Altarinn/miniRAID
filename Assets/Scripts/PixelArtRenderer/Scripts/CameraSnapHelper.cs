using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace PixelArtRenderer
{
    /// <summary>
    /// Helper class for camera auto-snap functionality to pixel-perfect positions
    /// </summary>
    public class CameraSnapHelper
    {
        private Dictionary<float, List<(float theta, float y)>> cache = new Dictionary<float, List<(float, float)>>();
        
        public int maxInteger = 8;
        public float yMin = 8f;
        public float yMax = 45f;
        
        /// <summary>
        /// Precomputes valid camera positions for a given offsetZ value
        /// </summary>
        /// <param name="offsetZ">The fixed Z offset of the camera</param>
        public void PrecomputeForOffset(float offsetZ)
        {
            if (cache.ContainsKey(Mathf.Abs(offsetZ))) return;
            
            var list = new List<(float, float)>();

            for (int n = 1; n <= maxInteger; n++)
            {
                for (int m = 1; m <= maxInteger; m++)
                {
                    // Allow negative numbers for a and b
                    float[] aChoices = { n, 1f / n, -n, -1f / n };
                    float[] bChoices = { m, 1f / m, -m, -1f / m };

                    foreach (float a in aChoices)
                    {
                        foreach (float b in bChoices)
                        {
                            float q = a * b;
                            if (!(q > 0 && q < 1)) continue;

                            float r = Mathf.Sqrt(q / (1 - q));
                            float y = r * offsetZ;
                            if (y < yMin || y > yMax) continue;

                            float s = Mathf.Sqrt(q);
                            float theta = Mathf.Atan(a / s) * Mathf.Rad2Deg;

                            // Include 90-degree variants (not just 180)
                            for (int k = -3; k <= 3; k++)
                            {
                                float ang = Mathf.Repeat(theta + 90f * k, 360f);
                                list.Add((ang, y));
                            }
                        }
                    }
                }
            }
            
            cache[offsetZ] = list;
        }
        
        /// <summary>
        /// Finds the nearest valid position to the current orbital camera parameters
        /// </summary>
        /// <param name="currentTheta">Current X rotation in degrees</param>
        /// <param name="currentY">Current Y offset</param>
        /// <param name="offsetZ">The fixed Z offset of the camera</param>
        /// <returns>Tuple containing target theta and y values</returns>
        public (float theta, float y) FindNearestValidPosition(float currentTheta, float currentY, float offsetZ)
        {
            if (!cache.ContainsKey(Mathf.Abs(offsetZ)))
            {
                PrecomputeForOffset(Mathf.Abs(offsetZ));
            }
            
            var list = cache[Mathf.Abs(offsetZ)];
            float bestDist = float.MaxValue;
            (float, float) best = (0, 0);

            // Calculate current world position
            Vector3 currentLocal = new Vector3(0, currentY, offsetZ);
            Vector3 currentWorld = Quaternion.Euler(0, currentTheta, 0) * currentLocal;

            foreach (var (theta, y) in list)
            {
                // Calculate candidate world position
                Vector3 candidateLocal = new Vector3(0, y, offsetZ);
                Vector3 candidateWorld = Quaternion.Euler(0, theta, 0) * candidateLocal;
                
                // World space distance
                float dist = (candidateWorld - currentWorld).sqrMagnitude;
                
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = (theta, y);
                }
            }
            
            return best;
        }
        
        /// <summary>
        /// Clears the precomputed cache for a specific offsetZ
        /// </summary>
        /// <param name="offsetZ">The Z offset to clear from cache</param>
        public void ClearCache(float offsetZ)
        {
            cache.Remove(Mathf.Abs(offsetZ));
        }
        
        /// <summary>
        /// Clears all precomputed cache
        /// </summary>
        public void ClearAllCache()
        {
            cache.Clear();
        }
    }
}