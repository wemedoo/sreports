namespace sReportsV2.Domain.Sql.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdatePermissions : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM Modules; DELETE FROM PermissionModules; DELETE FROM Roles; DELETE FROM UserRoles; DELETE FROM Permissions; DELETE FROM PermissionRoles;");
        }
        
        public override void Down()
        {
        }
    }
}
