using sReportsV2.DAL.Sql.Sql;
using System.Data.Entity.Migrations;

namespace sReportsV2.Domain.Sql
{
    public static class MigrationHelper
    {
        public static void SetSystemVersionedTables(this SReportsContext context, string tableName)
        {
            string addStartDateTimeColumn = $@"
                ALTER TABLE {tableName}
                ADD StartDateTime DATETIME2;";
            string addEndDateTimeColumn = $@"
                ALTER TABLE {tableName}
                ADD EndDateTime DATETIME2;";

            string updateStartDateTimeEndDataTime = $@"
                UPDATE {tableName} SET StartDateTime = '19000101 00:00:00.0000000', EndDateTime = '99991231 23:59:59.9999999';";

            string alterStartDateTimeColumn = $@"
                ALTER TABLE {tableName}
                ALTER COLUMN StartDateTime DATETIME2 NOT NULL;";
            string alterEndDateTimeColumn = $@"
                ALTER TABLE {tableName}
                ALTER COLUMN EndDateTime DATETIME2 NOT NULL;";


            string addPeriodForSystemTime = $@"
                ALTER TABLE {tableName}
                ADD PERIOD FOR SYSTEM_TIME (StartDateTime, EndDateTime);";
            string addSystemVersioningToTheTable = $@"
                ALTER TABLE {tableName}
                SET(SYSTEM_VERSIONING = ON (HISTORY_TABLE = {tableName}_History, DATA_CONSISTENCY_CHECK = ON));";

            context.Database.ExecuteSqlCommand(addStartDateTimeColumn);
            context.Database.ExecuteSqlCommand(addEndDateTimeColumn);
            context.Database.ExecuteSqlCommand(updateStartDateTimeEndDataTime);
            context.Database.ExecuteSqlCommand(alterStartDateTimeColumn);
            context.Database.ExecuteSqlCommand(alterEndDateTimeColumn);
            context.Database.ExecuteSqlCommand(addPeriodForSystemTime);
            context.Database.ExecuteSqlCommand(addSystemVersioningToTheTable);
        }

        public static void UnsetSystemVersionedTables(this SReportsContext context, string tableName)
        {
            string removeSystemVersioningFromTheTable = $@"
                ALTER TABLE {tableName} SET ( SYSTEM_VERSIONING = OFF);";
            string dropPeriodForSystemTime = $@"
                ALTER TABLE {tableName} DROP PERIOD FOR SYSTEM_TIME;";
            string dropHistoryTable = $@"
                DROP TABLE {tableName}_History;";

            string dropStartDateTime = $@"
                ALTER TABLE {tableName} DROP COLUMN StartDateTime;";
            string dropEndDateTime = $@"
                ALTER TABLE {tableName} DROP COLUMN EndDateTime;";

            context.Database.ExecuteSqlCommand(removeSystemVersioningFromTheTable);
            context.Database.ExecuteSqlCommand(dropPeriodForSystemTime);
            context.Database.ExecuteSqlCommand(dropHistoryTable);
            context.Database.ExecuteSqlCommand(dropStartDateTime);
            context.Database.ExecuteSqlCommand(dropEndDateTime);
        }

        public static void CreateIndexesOnCommonProperties(this SReportsContext context, string tableName)
        {
            string createLastUpdateIndex = $@"
                CREATE INDEX IX_LastUpdate
                ON {tableName} (LastUpdate);";
            string createStartDateTimeIndex = $@"
                CREATE INDEX IX_StartDateTime
                ON {tableName} (StartDateTime);";
            string createEndDateTimeIndex = $@"
                CREATE INDEX IX_EndDateTime
                ON {tableName} (EndDateTime);";

            context.Database.ExecuteSqlCommand(createLastUpdateIndex);
            context.Database.ExecuteSqlCommand(createStartDateTimeIndex);
            context.Database.ExecuteSqlCommand(createEndDateTimeIndex);
        }

        public static void DropIndexesOnCommonProperties(this SReportsContext context, string tableName)
        {
            string dropLastUpdateIndex = $@"
                DROP INDEX {tableName}.IX_LastUpdate;";
            string dropStartDateTimeIndex = $@"
                DROP INDEX {tableName}.IX_StartDateTime;";
            string dropEndDateTimeIndex = $@"
                DROP INDEX {tableName}.IX_EndDateTime;";

            context.Database.ExecuteSqlCommand(dropLastUpdateIndex);
            context.Database.ExecuteSqlCommand(dropStartDateTimeIndex);
            context.Database.ExecuteSqlCommand(dropEndDateTimeIndex);
        }

        public static void CreateCountryTempTableAndSaveData(this SReportsContext context, string tableName)
        {
            string createTempTable = $@"
                CREATE TABLE dbo.Country{tableName}TempTable (
                CustomEnumId int,
                AddressId int
            );";
            string saveDataInTempTable = $@"
                insert into dbo.Country{tableName}TempTable (CustomEnumId, AddressId) 
                select customEnum.Id customEnumId, 
	                a.Id addressId
                from dbo.{tableName} a
                inner join dbo.ThesaurusEntryTranslations translation 
                on translation.PreferredTerm = a.Country
                inner join dbo.CustomEnums customEnum 
                on customEnum.ThesaurusEntryId = translation.ThesaurusEntryId
                where customEnum.Type = 10
                group by customEnum.Id, a.Id
            ;";

            context.Database.ExecuteSqlCommand(createTempTable);
            context.Database.ExecuteSqlCommand(saveDataInTempTable);
        }

        public static void SetCountryDataFromTempTable(this SReportsContext context, string tableName)
        {
            string updateAddressTable = $@"
                update a set a.CountryId = temp.CustomEnumId
                from dbo.Country{tableName}TempTable temp
                inner join dbo.{tableName} a on a.Id = temp.AddressId
            ;";
            string dropTempTable = $@"
                drop table dbo.Country{tableName}TempTable;
            ";

            context.Database.ExecuteSqlCommand(updateAddressTable);
            context.Database.ExecuteSqlCommand(dropTempTable);
        }
    }
}
