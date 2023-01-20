namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public abstract class MongoMigration
    {
        public abstract int Version { get; }
        /// <summary>
        ///     Operations to be performed during the upgrade process.
        /// </summary>
        public abstract void Up();

        /// <summary>
        ///     Operations to be performed during the downgrade process.
        /// </summary>
        public abstract void Down();
    }
}
