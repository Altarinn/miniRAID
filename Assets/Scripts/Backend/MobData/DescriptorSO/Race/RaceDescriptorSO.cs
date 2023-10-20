using System.Collections.Generic;
using miniRAID.Buff;
using UnityEngine;
using UnityEngine.Serialization;

namespace miniRAID
{
    [CreateAssetMenu(fileName = "RaceDescriptor", menuName = "Race descriptor")]
    public class RaceDescriptorSO : CustomIconScriptableObject
    {
        public string raceName;
        
        public Consts.BaseStats baseGrowthModifier;
        
        public List<MobListenerSOEntry> racePassives;
        public List<ActionSOEntry> raceActions;
    }
}