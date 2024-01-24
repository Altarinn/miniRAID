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
        
        static private NumericalDatabase<TKey> _instance;
        static public NumericalDatabase<TKey> GetSingleton()
        {
            if (_instance == null)
            {
                _instance = new NumericalDatabase<TKey>();
            }
            return _instance;
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

        public void AddMatcher(NumericalDatabaseMatcherBase<TKey> matcher)
        {
            _matchers.Add(matcher);
        }
        
        public bool GetStat(object parent, TKey statName, out object result)
        {
            // if (cache.ContainsKey(statName))
            // {
            //     return (T)cache[statName].value;
            // }
            // else
            // {
            // Try to find the stat
            foreach (var matcher in _matchers)
            {
                if (matcher.MatchStat(parent, statName, out object o))
                {
                    result = o;
                    return true;
                }
            }
            
            // throw new System.Exception("Stat " + statName.ToString() + " not found in the numerical database.");
            result = null;
            return false;
            // }
        }
    }
}