using System;
using miniRAID.Backend.Numericals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Backend.NumericalsCore
{
    [Serializable]
    public struct FloatEntry
    {
        [PathBell]
        [SerializeField]
        [HideLabel]
        [ReadOnly]
        private float baseValue;
        
        [SerializeField]
        [HideLabel]
        private float multiplier;
        
        public static implicit operator float(FloatEntry d) => (float)(d.baseValue * d.multiplier);
    }
}