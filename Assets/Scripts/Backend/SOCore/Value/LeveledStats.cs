using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace miniRAID
{
    // public interface ILeveledObject
    // {
    //     public int Level { get; }
    //     public int MaxLevel { get; }
    // }

    [System.Serializable]
    public struct LeveledStats<T>
    {
        [SerializeField]
        public bool isLeveled;
        
        [SerializeField]
        public T[] values;
        
        public LeveledStats(T initialValue, int maxLevels = -1)
        {
            this.isLeveled = false;
            this.values = new T[maxLevels > 0 ? maxLevels : Consts.MaxListenerLevels];

            for(int i = 0; i < values.Length; i++)
            {
                values[i] = initialValue;
            }
        }
        
        public T Eval(int level)
        {
            if (isLeveled)
            {
                return values[level];
            }

            return values[0];
        }

        public bool IsValueNull()
        {
            return values == null;
        }
    }
}