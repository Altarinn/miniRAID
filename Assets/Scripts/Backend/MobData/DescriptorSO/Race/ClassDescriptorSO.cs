using System.Collections.Generic;
using miniRAID.Buff;
using UnityEngine;

namespace miniRAID
{
    [CreateAssetMenu(fileName = "ClassDescriptor", menuName = "Class descriptor")]
    public class ClassDescriptorSO : CustomIconScriptableObject
    {
        public string className;

        public Consts.BaseStatsInt baseStatsLv1;
        public Consts.BaseStats baseGrowth;
    
        public List<MobListenerSOEntry> classPassives;
        public List<ActionSOEntry> classActions;
    }
}