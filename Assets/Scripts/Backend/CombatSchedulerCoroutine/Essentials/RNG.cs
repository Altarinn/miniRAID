using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace miniRAID
{
    // TODO: Record my state at each time-step
    public class RNG
    {
        private Random random;

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
            return random.NextFloat() < p;
        }
    }
}