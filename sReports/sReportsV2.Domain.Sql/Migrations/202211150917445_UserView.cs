namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserView : DbMigration
    {
        public override void Up()
        {
			string script =
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
					,userOrg.[State]";

			SReportsContext sReportsContext = new SReportsContext();
			sReportsContext.Database.ExecuteSqlCommand(script);
		}
        
        public override void Down()
        {
        }
    }
}
