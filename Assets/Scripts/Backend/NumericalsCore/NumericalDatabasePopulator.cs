namespace miniRAID.Backend
{
    public abstract class NumericalDatabasePopulatorBase<TKey>
    {
        private NumericalDatabase<TKey> database;
        
        public NumericalDatabasePopulatorBase(NumericalDatabase<TKey> database)
        {
            this.database = database;
        }
        
        public abstract bool PopulateStat(TKey key);
    }

    public abstract class WildcardPopulator<TKey> : NumericalDatabasePopulatorBase<TKey>
    {
        protected WildcardPopulator(NumericalDatabase<TKey> database) : base(database)
        { }
        
        // TODO
    }
}