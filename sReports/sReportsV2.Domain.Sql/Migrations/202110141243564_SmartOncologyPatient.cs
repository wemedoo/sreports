namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SmartOncologyPatient : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Patients", name: "Addresss_Id", newName: "Address_Id");
            RenameIndex(table: "dbo.Patients", name: "IX_Addresss_Id", newName: "IX_Address_Id");
            CreateTable(
                "dbo.SmartOncologyPatients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdentificationNumber = c.String(),
                        Allergies = c.String(),
                        PatientInformedFor = c.String(),
                        PatientInformedBy = c.String(),
                        PatientInfoSignedOn = c.DateTime(),
                        CopyDeliveredOn = c.DateTime(),
                        CapabilityToWork = c.Int(nullable: false),
                        DesireToHaveChildren = c.Boolean(nullable: false),
                        FertilityConservation = c.Boolean(nullable: false),
                        SemenCryopreservation = c.Boolean(nullable: false),
                        EggCellCryopreservation = c.Boolean(nullable: false),
                        SexualHealthAddressed = c.Boolean(nullable: false),
                        Contraception = c.Int(nullable: false),
                        ClinicalTrials = c.String(),
                        PreviousTreatment = c.Boolean(nullable: false),
                        TreatmentInCantonalHospitalGraubunden = c.Boolean(nullable: false),
                        HistoryOfOncologicalDisease = c.String(),
                        HospitalOrPraxisOfPreviousTreatments = c.String(),
                        DiseaseContextAtInitialPresentation = c.Int(nullable: false),
                        StageAtInitialPresentation = c.String(),
                        DiseaseContextAtCurrentPresentation = c.Int(nullable: false),
                        StageAtCurrentPresentation = c.String(),
                        Anatomy = c.String(),
                        Morphology = c.String(),
                        TherapeuticContext = c.String(),
                        ChemotherapyType = c.String(),
                        ChemotherapyCourse = c.Int(nullable: false),
                        ChemotherapyCycle = c.Int(nullable: false),
                        FirstDayOfChemotherapy = c.DateTime(),
                        ConsecutiveChemotherapyDays = c.String(),
                        Active = c.Boolean(nullable: false),
                        Name_Given = c.String(),
                        Name_Family = c.String(),
                        Gender = c.Int(nullable: false),
                        BirthDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        Address_Id = c.Int(),
                        ContactPerson_Id = c.Int(),
                        MultipleB_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.Address_Id)
                .ForeignKey("dbo.Contacts", t => t.ContactPerson_Id)
                .ForeignKey("dbo.MultipleBirths", t => t.MultipleB_Id)
                .Index(t => t.Address_Id)
                .Index(t => t.ContactPerson_Id)
                .Index(t => t.MultipleB_Id);
            
            AddColumn("dbo.Communications", "SmartOncologyPatient_Id", c => c.Int());
            AddColumn("dbo.Telecoms", "SmartOncologyPatient_Id", c => c.Int());
            AddColumn("dbo.Identifiers", "SmartOncologyPatient_Id", c => c.Int());
            AddColumn("dbo.EpisodeOfCares", "SmartOncologyPatient_Id", c => c.Int());
            CreateIndex("dbo.Communications", "SmartOncologyPatient_Id");
            CreateIndex("dbo.Telecoms", "SmartOncologyPatient_Id");
            CreateIndex("dbo.Identifiers", "SmartOncologyPatient_Id");
            CreateIndex("dbo.EpisodeOfCares", "SmartOncologyPatient_Id");
            AddForeignKey("dbo.Communications", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients", "Id");
            AddForeignKey("dbo.EpisodeOfCares", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients", "Id");
            AddForeignKey("dbo.Identifiers", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients", "Id");
            AddForeignKey("dbo.Telecoms", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Telecoms", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients");
            DropForeignKey("dbo.SmartOncologyPatients", "MultipleB_Id", "dbo.MultipleBirths");
            DropForeignKey("dbo.Identifiers", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients");
            DropForeignKey("dbo.EpisodeOfCares", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients");
            DropForeignKey("dbo.SmartOncologyPatients", "ContactPerson_Id", "dbo.Contacts");
            DropForeignKey("dbo.Communications", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients");
            DropForeignKey("dbo.SmartOncologyPatients", "Address_Id", "dbo.Addresses");
            DropIndex("dbo.SmartOncologyPatients", new[] { "MultipleB_Id" });
            DropIndex("dbo.SmartOncologyPatients", new[] { "ContactPerson_Id" });
            DropIndex("dbo.SmartOncologyPatients", new[] { "Address_Id" });
            DropIndex("dbo.EpisodeOfCares", new[] { "SmartOncologyPatient_Id" });
            DropIndex("dbo.Identifiers", new[] { "SmartOncologyPatient_Id" });
            DropIndex("dbo.Telecoms", new[] { "SmartOncologyPatient_Id" });
            DropIndex("dbo.Communications", new[] { "SmartOncologyPatient_Id" });
            DropColumn("dbo.EpisodeOfCares", "SmartOncologyPatient_Id");
            DropColumn("dbo.Identifiers", "SmartOncologyPatient_Id");
            DropColumn("dbo.Telecoms", "SmartOncologyPatient_Id");
            DropColumn("dbo.Communications", "SmartOncologyPatient_Id");
            DropTable("dbo.SmartOncologyPatients");
            RenameIndex(table: "dbo.Patients", name: "IX_Address_Id", newName: "IX_Addresss_Id");
            RenameColumn(table: "dbo.Patients", name: "Address_Id", newName: "Addresss_Id");
        }
    }
}
