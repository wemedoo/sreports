namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        City = c.String(maxLength: 100),
                        State = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 10),
                        Country = c.String(maxLength: 50),
                        Street = c.String(maxLength: 200),
                        StreetNumber = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AdministrativeDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ThesaurusEntryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Versions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        RevokedOn = c.DateTime(),
                        State = c.Int(),
                        AdministrativeDataId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdministrativeDatas", t => t.AdministrativeDataId, cascadeDelete: true)
                .Index(t => t.AdministrativeDataId);
            
            CreateTable(
                "dbo.ClinicalDomains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CodeSystems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        Label = c.String(),
                        SAB = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommentState = c.Int(nullable: false),
                        Value = c.String(),
                        ItemRef = c.String(),
                        CommentRef = c.Int(),
                        FormRef = c.String(),
                        UserId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Communications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Language = c.String(),
                        Preferred = c.Boolean(nullable: false),
                        PatientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Relationship = c.String(),
                        Name_Given = c.String(),
                        Name_Family = c.String(),
                        Gender = c.String(),
                        Address_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.Address_Id)
                .Index(t => t.Address_Id);
            
            CreateTable(
                "dbo.Telecoms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        System = c.String(),
                        Value = c.String(),
                        Use = c.String(),
                        Contact_Id = c.Int(),
                        Organization_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.Contact_Id)
                .ForeignKey("dbo.Organizations", t => t.Organization_Id)
                .ForeignKey("dbo.Patients", t => t.Patient_Id)
                .Index(t => t.Contact_Id)
                .Index(t => t.Organization_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.CustomEnums",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ThesaurusEntryId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.ThesaurusEntries", t => t.ThesaurusEntryId, cascadeDelete: true)
                .Index(t => t.ThesaurusEntryId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Alias = c.String(),
                        PrimaryColor = c.String(),
                        SecondaryColor = c.String(),
                        LogoUrl = c.String(),
                        Email = c.String(),
                        Description = c.String(),
                        NumOfUsers = c.Int(nullable: false),
                        AddressId = c.Int(),
                        OrganizationRelationId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .ForeignKey("dbo.OrganizationRelations", t => t.OrganizationRelationId)
                .Index(t => t.AddressId)
                .Index(t => t.OrganizationRelationId);
            
            CreateTable(
                "dbo.OrganizationClinicalDomains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrganizationId = c.Int(nullable: false),
                        ClinicalDomainId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClinicalDomains", t => t.ClinicalDomainId, cascadeDelete: true)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .Index(t => t.OrganizationId)
                .Index(t => t.ClinicalDomainId);
            
            CreateTable(
                "dbo.Identifiers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        System = c.String(),
                        Value = c.String(),
                        Type = c.String(),
                        Use = c.String(),
                        Organization_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.Organization_Id)
                .ForeignKey("dbo.Patients", t => t.Patient_Id)
                .Index(t => t.Organization_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.OrganizationRelations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        ChildId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.ChildId)
                .ForeignKey("dbo.Organizations", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.ChildId);
            
            CreateTable(
                "dbo.ThesaurusEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        State = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        AdministrativeData_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdministrativeDatas", t => t.AdministrativeData_Id)
                .Index(t => t.AdministrativeData_Id);
            
            CreateTable(
                "dbo.O4CodeableConcept",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Version = c.String(),
                        Code = c.String(),
                        Value = c.String(),
                        Link = c.String(),
                        VersionPublishDate = c.DateTime(),
                        ThesaurusEntryId = c.Int(nullable: false),
                        EntryDateTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        CodeSystemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CodeSystems", t => t.CodeSystemId, cascadeDelete: true)
                .ForeignKey("dbo.ThesaurusEntries", t => t.ThesaurusEntryId, cascadeDelete: true)
                .Index(t => t.ThesaurusEntryId)
                .Index(t => t.CodeSystemId);
            
            CreateTable(
                "dbo.ThesaurusEntryTranslations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ThesaurusEntryId = c.Int(nullable: false),
                        Language = c.String(),
                        Definition = c.String(),
                        PreferredTerm = c.String(maxLength: 500, unicode: false),
                        SynonymsString = c.String(),
                        AbbreviationsString = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ThesaurusEntries", t => t.ThesaurusEntryId, cascadeDelete: true)
                .Index(t => t.ThesaurusEntryId);
            
            CreateTable(
                "dbo.Encounters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EpisodeOfCareId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Class = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        ServiceType = c.Int(nullable: false),
                        Period_Start = c.DateTime(nullable: false),
                        Period_End = c.DateTime(),
                        PatientId = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EpisodeOfCares",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        DiagnosisCondition = c.String(),
                        DiagnosisRole = c.Int(nullable: false),
                        DiagnosisRank = c.String(),
                        Period_Start = c.DateTime(nullable: false),
                        Period_End = c.DateTime(),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.EpisodeOfCareWorkflows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiagnosisCondition = c.String(),
                        DiagnosisRole = c.Int(nullable: false),
                        User = c.Int(nullable: false),
                        Submited = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        EpisodeOfCareId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EpisodeOfCares", t => t.EpisodeOfCareId, cascadeDelete: true)
                .Index(t => t.EpisodeOfCareId);
            
            CreateTable(
                "dbo.GlobalThesaurusUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Source = c.Int(),
                        Affiliation = c.String(),
                        Country = c.String(),
                        Phone = c.String(),
                        Password = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OutsideUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Institution = c.String(),
                        InstitutionAddress = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        Address_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.Address_Id)
                .Index(t => t.Address_Id);
            
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Active = c.Boolean(nullable: false),
                        Name_Given = c.String(),
                        Name_Family = c.String(),
                        Gender = c.Int(nullable: false),
                        BirthDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        Addresss_Id = c.Int(),
                        ContactPerson_Id = c.Int(),
                        MultipleB_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.Addresss_Id)
                .ForeignKey("dbo.Contacts", t => t.ContactPerson_Id)
                .ForeignKey("dbo.MultipleBirths", t => t.MultipleB_Id)
                .Index(t => t.Addresss_Id)
                .Index(t => t.ContactPerson_Id)
                .Index(t => t.MultipleB_Id);
            
            CreateTable(
                "dbo.MultipleBirths",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        isMultipleBorn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SimilarTerms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2950, unicode: false),
                        Definition = c.String(),
                        Source = c.Int(nullable: false),
                        EntryDateTime = c.DateTime(),
                        ThesaurusEntryTranslationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ThesaurusEntryTranslations", t => t.ThesaurusEntryTranslationId, cascadeDelete: true)
                .Index(t => t.ThesaurusEntryTranslationId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        DayOfBirth = c.DateTime(),
                        MiddleName = c.String(),
                        Email = c.String(),
                        ContactPhone = c.String(),
                        Prefix = c.Int(nullable: false),
                        AddressId = c.Int(),
                        UserConfigId = c.Int(nullable: false),
                        ActiveOrganizationId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.ActiveOrganizationId)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .ForeignKey("dbo.UserConfigs", t => t.UserConfigId, cascadeDelete: true)
                .Index(t => t.AddressId)
                .Index(t => t.UserConfigId)
                .Index(t => t.ActiveOrganizationId);
            
            CreateTable(
                "dbo.UserClinicalTrials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Acronym = c.String(),
                        SponosorId = c.String(),
                        WemedooId = c.String(),
                        Status = c.Int(),
                        Role = c.Int(),
                        IsArchived = c.Boolean(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserOrganizations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LegalForm = c.Int(),
                        OrganizationalForm = c.Int(),
                        DepartmentName = c.String(),
                        Team = c.String(),
                        IsPracticioner = c.Boolean(),
                        Qualification = c.String(),
                        SeniorityLevel = c.String(),
                        Speciality = c.String(),
                        SubSpeciality = c.String(),
                        State = c.Int(),
                        UserId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserOrganization_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserOrganizations", t => t.UserOrganization_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.UserOrganization_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserConfigs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PageSize = c.Int(nullable: false),
                        ActiveLanguage = c.String(),
                        TimeZoneOffset = c.String(),
                        ActiveOrganizationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.ActiveOrganizationId)
                .Index(t => t.ActiveOrganizationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "UserConfigId", "dbo.UserConfigs");
            DropForeignKey("dbo.UserConfigs", "ActiveOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Roles", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserOrganizations", "UserId", "dbo.Users");
            DropForeignKey("dbo.Roles", "UserOrganization_Id", "dbo.UserOrganizations");
            DropForeignKey("dbo.UserOrganizations", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.UserClinicalTrials", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Users", "ActiveOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.SimilarTerms", "ThesaurusEntryTranslationId", "dbo.ThesaurusEntryTranslations");
            DropForeignKey("dbo.Telecoms", "Patient_Id", "dbo.Patients");
            DropForeignKey("dbo.Patients", "MultipleB_Id", "dbo.MultipleBirths");
            DropForeignKey("dbo.Identifiers", "Patient_Id", "dbo.Patients");
            DropForeignKey("dbo.EpisodeOfCares", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Patients", "ContactPerson_Id", "dbo.Contacts");
            DropForeignKey("dbo.Communications", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Patients", "Addresss_Id", "dbo.Addresses");
            DropForeignKey("dbo.OutsideUsers", "Address_Id", "dbo.Addresses");
            DropForeignKey("dbo.EpisodeOfCareWorkflows", "EpisodeOfCareId", "dbo.EpisodeOfCares");
            DropForeignKey("dbo.CustomEnums", "ThesaurusEntryId", "dbo.ThesaurusEntries");
            DropForeignKey("dbo.ThesaurusEntryTranslations", "ThesaurusEntryId", "dbo.ThesaurusEntries");
            DropForeignKey("dbo.O4CodeableConcept", "ThesaurusEntryId", "dbo.ThesaurusEntries");
            DropForeignKey("dbo.O4CodeableConcept", "CodeSystemId", "dbo.CodeSystems");
            DropForeignKey("dbo.ThesaurusEntries", "AdministrativeData_Id", "dbo.AdministrativeDatas");
            DropForeignKey("dbo.CustomEnums", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Telecoms", "Organization_Id", "dbo.Organizations");
            DropForeignKey("dbo.Organizations", "OrganizationRelationId", "dbo.OrganizationRelations");
            DropForeignKey("dbo.OrganizationRelations", "ParentId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationRelations", "ChildId", "dbo.Organizations");
            DropForeignKey("dbo.Identifiers", "Organization_Id", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationClinicalDomains", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationClinicalDomains", "ClinicalDomainId", "dbo.ClinicalDomains");
            DropForeignKey("dbo.Organizations", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Telecoms", "Contact_Id", "dbo.Contacts");
            DropForeignKey("dbo.Contacts", "Address_Id", "dbo.Addresses");
            DropForeignKey("dbo.Versions", "AdministrativeDataId", "dbo.AdministrativeDatas");
            DropIndex("dbo.UserConfigs", new[] { "ActiveOrganizationId" });
            DropIndex("dbo.Roles", new[] { "User_Id" });
            DropIndex("dbo.Roles", new[] { "UserOrganization_Id" });
            DropIndex("dbo.UserOrganizations", new[] { "OrganizationId" });
            DropIndex("dbo.UserOrganizations", new[] { "UserId" });
            DropIndex("dbo.UserClinicalTrials", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "ActiveOrganizationId" });
            DropIndex("dbo.Users", new[] { "UserConfigId" });
            DropIndex("dbo.Users", new[] { "AddressId" });
            DropIndex("dbo.SimilarTerms", new[] { "ThesaurusEntryTranslationId" });
            DropIndex("dbo.Patients", new[] { "MultipleB_Id" });
            DropIndex("dbo.Patients", new[] { "ContactPerson_Id" });
            DropIndex("dbo.Patients", new[] { "Addresss_Id" });
            DropIndex("dbo.OutsideUsers", new[] { "Address_Id" });
            DropIndex("dbo.EpisodeOfCareWorkflows", new[] { "EpisodeOfCareId" });
            DropIndex("dbo.EpisodeOfCares", new[] { "PatientId" });
            DropIndex("dbo.ThesaurusEntryTranslations", new[] { "ThesaurusEntryId" });
            DropIndex("dbo.O4CodeableConcept", new[] { "CodeSystemId" });
            DropIndex("dbo.O4CodeableConcept", new[] { "ThesaurusEntryId" });
            DropIndex("dbo.ThesaurusEntries", new[] { "AdministrativeData_Id" });
            DropIndex("dbo.OrganizationRelations", new[] { "ChildId" });
            DropIndex("dbo.OrganizationRelations", new[] { "ParentId" });
            DropIndex("dbo.Identifiers", new[] { "Patient_Id" });
            DropIndex("dbo.Identifiers", new[] { "Organization_Id" });
            DropIndex("dbo.OrganizationClinicalDomains", new[] { "ClinicalDomainId" });
            DropIndex("dbo.OrganizationClinicalDomains", new[] { "OrganizationId" });
            DropIndex("dbo.Organizations", new[] { "OrganizationRelationId" });
            DropIndex("dbo.Organizations", new[] { "AddressId" });
            DropIndex("dbo.CustomEnums", new[] { "OrganizationId" });
            DropIndex("dbo.CustomEnums", new[] { "ThesaurusEntryId" });
            DropIndex("dbo.Telecoms", new[] { "Patient_Id" });
            DropIndex("dbo.Telecoms", new[] { "Organization_Id" });
            DropIndex("dbo.Telecoms", new[] { "Contact_Id" });
            DropIndex("dbo.Contacts", new[] { "Address_Id" });
            DropIndex("dbo.Communications", new[] { "PatientId" });
            DropIndex("dbo.Versions", new[] { "AdministrativeDataId" });
            DropTable("dbo.UserConfigs");
            DropTable("dbo.Roles");
            DropTable("dbo.UserOrganizations");
            DropTable("dbo.UserClinicalTrials");
            DropTable("dbo.Users");
            DropTable("dbo.SimilarTerms");
            DropTable("dbo.MultipleBirths");
            DropTable("dbo.Patients");
            DropTable("dbo.OutsideUsers");
            DropTable("dbo.GlobalThesaurusUsers");
            DropTable("dbo.EpisodeOfCareWorkflows");
            DropTable("dbo.EpisodeOfCares");
            DropTable("dbo.Encounters");
            DropTable("dbo.ThesaurusEntryTranslations");
            DropTable("dbo.O4CodeableConcept");
            DropTable("dbo.ThesaurusEntries");
            DropTable("dbo.OrganizationRelations");
            DropTable("dbo.Identifiers");
            DropTable("dbo.OrganizationClinicalDomains");
            DropTable("dbo.Organizations");
            DropTable("dbo.CustomEnums");
            DropTable("dbo.Telecoms");
            DropTable("dbo.Contacts");
            DropTable("dbo.Communications");
            DropTable("dbo.Comments");
            DropTable("dbo.CodeSystems");
            DropTable("dbo.ClinicalDomains");
            DropTable("dbo.Versions");
            DropTable("dbo.AdministrativeDatas");
            DropTable("dbo.Addresses");
        }
    }
}
