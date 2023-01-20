namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChemotherapySchemaDefinition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BodySurfaceCalculationFormulas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Formula = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChemotherapySchemas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LengthOfCycle = c.Int(nullable: false),
                        NumOfCycles = c.Int(nullable: false),
                        AreCoursesLimited = c.Boolean(nullable: false),
                        NumOfLimitedCourses = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        Creator_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Creator_Id)
                .Index(t => t.Creator_Id);
            
            CreateTable(
                "dbo.Indications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ChemotherapySchema_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChemotherapySchemas", t => t.ChemotherapySchema_Id)
                .Index(t => t.ChemotherapySchema_Id);
            
            CreateTable(
                "dbo.LiteratureReferences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PubMedLink = c.String(),
                        PubMedID = c.Int(nullable: false),
                        ShortReferenceNotation = c.String(),
                        DOI = c.String(),
                        PublicationDate = c.DateTime(),
                        ChemotherapySchemaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChemotherapySchemas", t => t.ChemotherapySchemaId, cascadeDelete: true)
                .Index(t => t.ChemotherapySchemaId);
            
            CreateTable(
                "dbo.Medications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Dose = c.String(),
                        Unit = c.String(),
                        PreparationInstruction = c.String(),
                        ApplicationInstruction = c.String(),
                        AdditionalInstruction = c.String(),
                        RouteOfAdministration = c.Int(nullable: false),
                        BodySurfaceCalculationFormula = c.Int(nullable: false),
                        SameDoseForEveryAplication = c.Boolean(nullable: false),
                        HasMaximalCumulativeDose = c.Boolean(nullable: false),
                        CumulativeDose = c.String(),
                        CumulativeDoseUnit = c.String(),
                        WeekendHolidaysExcluded = c.Boolean(nullable: false),
                        MaxDayNumberOfApplicationiDelay = c.Int(),
                        IsSupportiveMedication = c.Boolean(nullable: false),
                        SupportiveMedicationReserve = c.Boolean(nullable: false),
                        SupportiveMedicationAlternative = c.Boolean(nullable: false),
                        ChemotherapySchemaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChemotherapySchemas", t => t.ChemotherapySchemaId, cascadeDelete: true)
                .Index(t => t.ChemotherapySchemaId);
            
            CreateTable(
                "dbo.MedicationDoses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DayNumber = c.Int(nullable: false),
                        Interval = c.String(),
                        StartsAt = c.String(),
                        Unit = c.String(),
                        MedicationId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Medications", t => t.MedicationId, cascadeDelete: true)
                .Index(t => t.MedicationId);
            
            CreateTable(
                "dbo.MedicationDoseTimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Time = c.String(),
                        Dose = c.String(),
                        MedicationDoseId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MedicationDoses", t => t.MedicationDoseId, cascadeDelete: true)
                .Index(t => t.MedicationDoseId);
            
            CreateTable(
                "dbo.MedicationDoseTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        StartAt = c.String(),
                        Intervals = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RouteOfAdministrations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Definition = c.String(),
                        ShortName = c.String(),
                        FDACode = c.String(),
                        NCICondeptId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Medications", "ChemotherapySchemaId", "dbo.ChemotherapySchemas");
            DropForeignKey("dbo.MedicationDoses", "MedicationId", "dbo.Medications");
            DropForeignKey("dbo.MedicationDoseTimes", "MedicationDoseId", "dbo.MedicationDoses");
            DropForeignKey("dbo.LiteratureReferences", "ChemotherapySchemaId", "dbo.ChemotherapySchemas");
            DropForeignKey("dbo.Indications", "ChemotherapySchema_Id", "dbo.ChemotherapySchemas");
            DropForeignKey("dbo.ChemotherapySchemas", "Creator_Id", "dbo.Users");
            DropIndex("dbo.MedicationDoseTimes", new[] { "MedicationDoseId" });
            DropIndex("dbo.MedicationDoses", new[] { "MedicationId" });
            DropIndex("dbo.Medications", new[] { "ChemotherapySchemaId" });
            DropIndex("dbo.LiteratureReferences", new[] { "ChemotherapySchemaId" });
            DropIndex("dbo.Indications", new[] { "ChemotherapySchema_Id" });
            DropIndex("dbo.ChemotherapySchemas", new[] { "Creator_Id" });
            DropTable("dbo.RouteOfAdministrations");
            DropTable("dbo.MedicationDoseTypes");
            DropTable("dbo.MedicationDoseTimes");
            DropTable("dbo.MedicationDoses");
            DropTable("dbo.Medications");
            DropTable("dbo.LiteratureReferences");
            DropTable("dbo.Indications");
            DropTable("dbo.ChemotherapySchemas");
            DropTable("dbo.BodySurfaceCalculationFormulas");
        }
    }
}
