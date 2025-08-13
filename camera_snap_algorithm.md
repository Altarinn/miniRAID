**Camera Auto-Snap Algorithm Summary**

### 1. Mathematical Derivation

- **Parameters:**
  - `offsetZ` (fixed) = `m_FollowOffset.z`
  - `y` (variable) = `m_FollowOffset.y`
  - `theta` (variable) = `m_XAxis.Value` (degrees)
- Define:
  - `r = y / offsetZ`
  - `phi = atan(r)`
  - `s = sin(phi) = r / sqrt(1 + r^2)`
  - `t = tan(theta)`, `cot(theta) = 1/t`
- **Validity Condition:**
  - Both `t * s` and `(1/t) * s` must be either `A` or `1/A` where `A` is an integer.
- Let `a = t * s`, `b = (1/t) * s`.
  - Then `a * b = s^2`.
- If `a ∈ {n, 1/n}`, `b ∈ {m, 1/m}` for integers `n, m`:
  - `q = a * b ∈ {nm, n/m, m/n, 1/(nm)}`
  - Must have `0 < q < 1`.
- From `q = s^2`:
  - `r = sqrt(q / (1 - q))`
  - `y = offsetZ * r`
  - `tan(theta) = a / sqrt(q)` → `theta = atan(a / sqrt(q))`

### 2. Candidate Generation

1. Enumerate `n, m` from `1` to `maxInt`.
2. For each `(n, m)`, try both forms of `a` and `b`.
3. Compute `q`, check `0 < q < 1`.
4. Compute `y` and check clamp limits.
5. Compute `theta` and normalize (also try ±180° variants).
6. Store `(theta, y)` as valid position.

### 3. Runtime Nearest Search

- At runtime, when input is released:
  1. Look up the **precomputed** valid positions for the camera's `offsetZ`.
  2. Compute the world position for each candidate.
  3. Pick the one with smallest world-space distance to current camera.
  4. Smoothly interpolate `m_XAxis.Value` and `m_FollowOffset.y` to target.

---

**Simplified Implementation Plan:**

- Precompute valid `(theta, y)` pairs **once** per distinct `offsetZ` value (store in dictionary).
- Provide a method `FindNearestValidPosition(offsetZ, curPos)`.
- Use only `ToggleRotateCamera` to check user interaction.
- Ignore `charCamOrbital` for now.
- Keep a coroutine for smooth snapping.

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSnapSimple : MonoBehaviour
{
    public CinemachineVirtualCamera mainVCam;
    public float snapDuration = 0.5f;
    public int maxInteger = 8;
    public float yMin = 8f, yMax = 45f;

    CinemachineOrbitalTransposer mainOrbital;
    Dictionary<float, List<(float theta, float y)>> cache = new();
    Coroutine snapRoutine;

    void Start()
    {
        mainOrbital = mainVCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        PrecomputeForOffset(mainOrbital.m_FollowOffset.z);
    }

    void Update()
    {
        bool userInput = inputs.UI.ToggleRotateCamera.ReadValue<float>() > 0.5f;

        if (!userInput && snapRoutine == null)
        {
            snapRoutine = StartCoroutine(SnapToNearest());
        }
        if (userInput && snapRoutine != null)
        {
            StopCoroutine(snapRoutine);
            snapRoutine = null;
        }
    }

    void PrecomputeForOffset(float offsetZ)
    {
        if (cache.ContainsKey(offsetZ)) return;
        var list = new List<(float, float)>();

        for (int n = 1; n <= maxInteger; n++)
        for (int m = 1; m <= maxInteger; m++)
        {
            float[] aChoices = { n, 1f / n };
            float[] bChoices = { m, 1f / m };

            foreach (float a in aChoices)
            foreach (float b in bChoices)
            {
                float q = a * b;
                if (!(q > 0 && q < 1)) continue;

                float r = Mathf.Sqrt(q / (1 - q));
                float y = r * offsetZ;
                if (y < yMin || y > yMax) continue;

                float s = Mathf.Sqrt(q);
                float theta = Mathf.Atan(a / s) * Mathf.Rad2Deg;

                for (int k = -1; k <= 1; k++)
                {
                    float ang = Mathf.Repeat(theta + 180f * k, 360f);
                    list.Add((ang, y));
                }
            }
        }
        cache[offsetZ] = list;
    }

    (float theta, float y) FindNearestValidPosition(float offsetZ, Vector3 camPos, Vector3 followPos)
    {
        var list = cache[offsetZ];
        float bestDist = float.MaxValue;
        (float, float) best = (0, 0);

        foreach (var (theta, y) in list)
        {
            Vector3 local = new(0, y, offsetZ);
            Vector3 world = followPos + Quaternion.Euler(0, theta, 0) * local;
            float dist = (world - camPos).sqrMagnitude;
            if (dist < bestDist)
            {
                bestDist = dist;
                best = (theta, y);
            }
        }
        return best;
    }

    IEnumerator SnapToNearest()
    {
        float offsetZ = mainOrbital.m_FollowOffset.z;
        var (targetTheta, targetY) = FindNearestValidPosition(
            offsetZ, mainVCam.transform.position, mainOrbital.FollowTarget.position);

        float startTheta = mainOrbital.m_XAxis.Value;
        float startY = mainOrbital.m_FollowOffset.y;
        float delta = Mathf.DeltaAngle(startTheta, targetTheta);

        float t = 0f;
        while (t < snapDuration)
        {
            t += Time.deltaTime;
            float u = Mathf.SmoothStep(0, 1, t / snapDuration);
            mainOrbital.m_XAxis.Value = startTheta + delta * u;
            var off = mainOrbital.m_FollowOffset;
            off.y = Mathf.Lerp(startY, targetY, u);
            mainOrbital.m_FollowOffset = off;
            yield return null;
        }
        mainOrbital.m_XAxis.Value = targetTheta;
        var finalOff = mainOrbital.m_FollowOffset;
        finalOff.y = targetY;
        mainOrbital.m_FollowOffset = finalOff;
        snapRoutine = null;
    }
}
```

**Key changes from full version:**

- No charCam handling.
- Precomputation done once per offsetZ.
- Much fewer parameters, easier to follow.
- Preserved `FindNearestValidPosition` API for reuse with other cams.

