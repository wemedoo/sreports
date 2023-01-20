namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System.Data.Entity.Migrations;

    public partial class DbNamingConvention2 : DbMigration
    {
        public override void Up()
        {
            string dropScriptUp = @"DROP VIEW dbo.UserViews";

            string createScriptUp =
                @"CREATE OR ALTER VIEW dbo.UserViews
                    AS
					select
					users.[UserId]
					,users.[Username]
					,users.[Password]
					,users.[FirstName]
					,users.[LastName]
					,users.[DayOfBirth]
				    ,users.[MiddleName]
				    ,users.[Email]
				    ,users.[ContactPhone]
				    ,users.[Prefix]
				    ,users.[AddressId]
				    ,users.[UserConfigId]
				    ,users.[IsDeleted]
				    ,users.[RowVersion]
				    ,users.[EntryDatetime]
				    ,users.[LastUpdate]
				    ,users.[PersonalEmail]
				    ,users.[Salt]
					,userOrg.[OrganizationId]
					,userOrg.[State]
					,users.[Active]
					,users.[CreatedById]
					,STUFF((SELECT ', ' + roles.Name 
							FROM dbo.Roles roles
							INNER JOIN dbo.UserRoles userRoles 
							ON userRoles.RoleId = roles.RoleId
							WHERE users.UserId = userRoles.UserId
							FOR XML PATH('')), 1, 1, '') as Roles
					,STUFF((SELECT ', ' + org.Name 
							FROM dbo.Organizations org
							INNER JOIN dbo.UserOrganizations userOrg 
							ON userOrg.OrganizationId = org.OrganizationId
							WHERE users.UserId = userOrg.UserId
							FOR XML PATH('')), 1, 1, '') as UserOrganizations
					from dbo.Users users
					left join dbo.[UserRoles] userRoles
                    on userRoles.UserId = users.UserId
					left join dbo.[Roles] roles
					on userRoles.RoleId = roles.RoleId
					left join dbo.[UserOrganizations] userOrg
                    on userOrg.UserId = users.UserId
					left join dbo.[Organizations] org
					on userOrg.OrganizationId = org.OrganizationId
					group by users.[UserId]
					,users.[Username]
					,users.[Password]
					,users.[FirstName]
					,users.[LastName]
					,users.[DayOfBirth]
				    ,users.[MiddleName]
				    ,users.[Email]
				    ,users.[ContactPhone]
				    ,users.[Prefix]
				    ,users.[AddressId]
				    ,users.[UserConfigId]
				    ,users.[IsDeleted]
				    ,users.[RowVersion]
				    ,users.[EntryDatetime]
				    ,users.[LastUpdate]
				    ,users.[PersonalEmail]
				    ,users.[Salt]
					,userOrg.[OrganizationId]
					,userOrg.[State]
					,users.[Active]
					,users.[CreatedById]
				";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(dropScriptUp);
            sReportsContext.Database.ExecuteSqlCommand(createScriptUp);
        }

        public override void Down()
        {
            string dropScriptDown = @"DROP VIEW dbo.UserViews";

            string createScriptDown =
                @"CREATE OR ALTER VIEW dbo.UserViews
                    AS
					select
					users.[Id]
					,users.[Username]
					,users.[Password]
					,users.[FirstName]
					,users.[LastName]
					,users.[DayOfBirth]
				    ,users.[MiddleName]
				    ,users.[Email]
				    ,users.[ContactPhone]
				    ,users.[Prefix]
				    ,users.[AddressId]
				    ,users.[UserConfigId]
				    ,users.[IsDeleted]
				    ,users.[RowVersion]
				    ,users.[EntryDatetime]
				    ,users.[LastUpdate]
				    ,users.[PersonalEmail]
				    ,users.[Salt]
					,userOrg.[OrganizationId]
					,userOrg.[State]
					,users.[Active]
					,users.[CreatedById]
					,STUFF((SELECT ', ' + roles.Name 
							FROM dbo.Roles roles
							INNER JOIN dbo.UserRoles userRoles 
							ON userRoles.RoleId = roles.Id
							WHERE users.Id = userRoles.UserId
							FOR XML PATH('')), 1, 1, '') as Roles
					,STUFF((SELECT ', ' + org.Name 
							FROM dbo.Organizations org
							INNER JOIN dbo.UserOrganizations userOrg 
							ON userOrg.OrganizationId = org.Id
							WHERE users.Id = userOrg.UserId
							FOR XML PATH('')), 1, 1, '') as UserOrganizations
					from dbo.Users users
					left join dbo.[UserRoles] userRoles
                    on userRoles.UserId = users.Id
					left join dbo.[Roles] roles
					on userRoles.RoleId = roles.Id
					left join dbo.[UserOrganizations] userOrg
                    on userOrg.UserId = users.Id
					left join dbo.[Organizations] org
					on userOrg.OrganizationId = org.Id
					group by users.[Id]
					,users.[Username]
					,users.[Password]
					,users.[FirstName]
					,users.[LastName]
					,users.[DayOfBirth]
				    ,users.[MiddleName]
				    ,users.[Email]
				    ,users.[ContactPhone]
				    ,users.[Prefix]
				    ,users.[AddressId]
				    ,users.[UserConfigId]
				    ,users.[IsDeleted]
				    ,users.[RowVersion]
				    ,users.[EntryDatetime]
				    ,users.[LastUpdate]
				    ,users.[PersonalEmail]
				    ,users.[Salt]
					,userOrg.[OrganizationId]
					,userOrg.[State]
					,users.[Active]
					,users.[CreatedById]
				";

            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.Database.ExecuteSqlCommand(dropScriptDown);
            sReportsContext.Database.ExecuteSqlCommand(createScriptDown);
        }
    }
}
