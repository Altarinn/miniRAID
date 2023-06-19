using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace miniRAID
{
    // TODO: Record my state at each time-step
    public class RNG
    {
        private Random random;

        public struct RNGHistoryEntry
        {
            [DisplayAsString]
            public float probability;

            [DisplayAsString] public float roll;
            
            [DisplayAsString]
            public bool result;
        }

        public List<RNGHistoryEntry> history = new List<RNGHistoryEntry>();

        public RNG(uint seed = 42)
        {
            random = new Random(seed);
        }
        
        public int NextInt()
        {
            return random.NextInt();
        }

        public bool WithProbability(float p, bool passOnDefault = true)
        {
            float roll = random.NextFloat();
            history.Add(new RNGHistoryEntry
            {
                probability = p,
                roll = roll,
                result = roll < p
            });
            
            return roll < p;
        }
    }
}