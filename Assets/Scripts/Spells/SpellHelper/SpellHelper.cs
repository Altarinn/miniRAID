using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using XLua;

using System.Linq;

using Sirenix.OdinInspector;
using miniRAID.Spells;
using UnityEngine.PlayerLoop;

namespace miniRAID
{
    public static class SpellHelper
    {
        // public static IEnumerator SummonAtGrid(Mob source, Mob summonPrefab, Vector2Int position, System.Func<Mob, IEnumerator> initialization = null)
        // {
        //     var resolvedPosition = Globals.backend.FindNearestEmptyGrid(position);
        //
        //     var summoned = GameObject.Instantiate(summonPrefab.gameObject, Globals.backend.GridToWorldPos(position) + Vector2.one * 0.5f, Quaternion.identity).GetComponent<Mob>();
        //     
        //     yield return new JumpIn(initialization?.Invoke((summoned)));
        // }
    }
}
