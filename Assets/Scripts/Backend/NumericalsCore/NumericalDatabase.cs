using System.Collections.Generic;

namespace miniRAID.Backend
{
    public struct NumericEntry
    {
        public System.Type type;
        public object value;
    }

    public class NumericalDatabase<TKey>
    {
        // public Dictionary<TKey, NumericEntry> cache = new Dictionary<TKey, NumericEntry>();
        private List<NumericalDatabaseMatcherBase<TKey>> _matchers = new List<NumericalDatabaseMatcherBase<TKey>>();
        
        static private NumericalDatabase<TKey> instance;
        static public NumericalDatabase<TKey> GetSingleton()
        {
            if (instance == null)
            {
                instance = new NumericalDatabase<TKey>();
            }
            return instance;
        }

        // public bool StoreStat<T>(TKey key, T stat)
        // {
        //     // All entries are treated as constants so we can't change them once they are set.
        //     if (cache.ContainsKey(key))
        //     {
        //         return false;
        //     }
        //     
        //     cache.Add(key, new NumericEntry()
        //     {
        //         type = typeof(T),
        //         value = stat
        //     });
        //     
        //     return true;
        // }
        
        public bool GetStat(object parent, TKey statName, out object result)
        {
            // if (cache.ContainsKey(statName))
            // {
            //     return (T)cache[statName].value;
            // }
            // else
            // {
            // Try to populate the stat
            foreach (var populator in _matchers)
            {
                if (populator.PopulateStat(statName, out object o))
                {
                    result = o;
                    return true;
                }
            }
            
            // If no populator can populate the stat, return default value
            // throw new System.Exception("Stat " + statName.ToString() + " not found in the numerical database.");
            result = null;
            return false;
            // }
        }
    }
}