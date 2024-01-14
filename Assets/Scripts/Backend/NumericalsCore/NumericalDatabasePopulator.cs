namespace miniRAID.Backend
{
    public abstract class NumericalDatabaseMatcherBase<TKey>
    {
        // private NumericalDatabase<TKey> database;
        
        // public NumericalDatabaseMatcherBase(NumericalDatabase<TKey> database)
        // {
            // this.database = database;
        // }
        
        public abstract bool MatchStat(object parent, TKey key, out object value);
    }

    public abstract class WildcardMatcher<TKey> : NumericalDatabaseMatcherBase<TKey>
    {
        // protected WildcardMatcher(NumericalDatabase<TKey> database) : base(database)
        // { }
        
        // TODO
    }
}