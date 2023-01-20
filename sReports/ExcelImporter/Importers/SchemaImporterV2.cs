using ExcelImporter.Classes;
using ExcelImporter.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using ChemotherapySchemaExcel = ExcelImporter.Classes.ChemotherapySchema;
using ChemotherapySchemaDB = sReportsV2.Domain.Sql.Entities.ChemotherapySchema.ChemotherapySchema;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Interfaces;

namespace ExcelImporter.Importers
{
    public class SchemaImporterV2 : ExcelSaxImporter<ChemotherapySchemaDB>
    {
        private readonly IChemotherapySchemaDAL chemotherapySchemaDAL;
        private readonly IRouteOfAdministrationDAL routeOfAdministrationDAL;
        private readonly IUnitDAL unitDAL;
        private readonly int creatorId;

        public SchemaImporterV2(string fileName, string sheetName, IChemotherapySchemaDAL chemotherapySchemaDAL, IRouteOfAdministrationDAL routeOfAdministrationDAL, IUnitDAL unitDAL, int creatorId) : base(fileName, sheetName)
        {
            this.chemotherapySchemaDAL = chemotherapySchemaDAL;
            this.routeOfAdministrationDAL = routeOfAdministrationDAL;
            this.unitDAL = unitDAL;
            this.creatorId = creatorId;
        }

        public override void ImportDataFromExcelToDatabase()
        {
            if (chemotherapySchemaDAL.GetAllCount() == 0)
            {
                List<ChemotherapySchemaDB> chemotherapySchemaEntries = ImportFromExcel();
                InsertDataIntoDatabase(chemotherapySchemaEntries);
            }
        }

        protected override void InsertDataIntoDatabase(List<ChemotherapySchemaDB> entries)
        {
            chemotherapySchemaDAL.InsertMany(entries);
        }

        protected override List<ChemotherapySchemaDB> ImportFromExcel()
        {
            IEnumerable<ChemotherapySchemaExcel> schemas = ImportRowsFromExcel().Select(x => GetChemotherapySchemaExcel(x));
            List<ChemotherapySchemaDB> chemotherapySchemaEntries = GetChemotherapySchemas(schemas);
            return chemotherapySchemaEntries;
        }

        private ChemotherapySchemaExcel GetChemotherapySchemaExcel(RowInfo dataRow)
        {
            return new ChemotherapySchemaExcel()
            {
                Amount = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.Amount)),
                ApplicationInstruction = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.ApplicationInstruction)),
                HasMaximalCumulative = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.HasMaximalCumulative)),
                IndicationAnatomicalTumorName = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.IndicationAnatomicalTumorName)),
                IndicationMorphologicalTumorType = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.IndicationMorphologicalTumorType)),
                IndicationTreatmentIntent = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.IndicationTreatmentIntent)),
                IndicationTumorStage = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.IndicationTumorStage)),
                IsDoseSame = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.IsDoseSame)),
                IsSupportiveMedication = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.IsSupportiveMedication)),
                LiteratureReference = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.LiteratureReference)),
                MaximalCumulativeDose = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.MaximalCumulativeDose)),
                MedicationName = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.MedicationName)),
                NumberOfDays = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.NumberOfDays)),
                PremedicationDays = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.PremedicationDays)),
                PreparationInstruction = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.PreparationInstruction)),
                RouteOfAdministration = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.RouteOfAdministration)),
                SchemaName = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.SchemaName)),
                Unit = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaV2Constants.Unit))
            };
        }

        private List<ChemotherapySchemaDB> GetChemotherapySchemas(IEnumerable<ChemotherapySchemaExcel> schemas)
        {
            List<RouteOfAdministration> routeOfAdministrations = routeOfAdministrationDAL.FilterByName(string.Empty).ToList();
            var groupedSchemas = schemas.GroupBy(x => new
            {
                x.SchemaName,
                x.IndicationAnatomicalTumorName,
                x.IndicationMorphologicalTumorType,
                x.IndicationTumorStage,
                x.IndicationTreatmentIntent
            });

            List<ChemotherapySchemaDB> chemotherapySchemas = new List<ChemotherapySchemaDB>();
            foreach (var schema in groupedSchemas)
            {
                if (string.IsNullOrWhiteSpace(schema.Key.SchemaName))
                {
                    continue;
                }
                if (chemotherapySchemaDAL.FilterByName(schema.Key.SchemaName).Count() > 0)
                {
                    continue;
                }
                List<Medication> medications = new List<Medication>();
                var groupedMedications = schema.GroupBy(x => new
                {
                    x.MedicationName,
                    x.IsSupportiveMedication
                });
                foreach (var medication in groupedMedications)
                {
                    int i = 1;
                    var first = medication.FirstOrDefault();
                    Medication m = new Medication()
                    {
                        Name = medication.Key.MedicationName,
                        IsSupportiveMedication = medication.Key.IsSupportiveMedication?.ToLower()?.Equals("yes") ?? false,
                        UnitId = unitDAL.GetByNameId(first.Unit),
                        Dose = first.Amount,
                        RouteOfAdministration = routeOfAdministrations.FirstOrDefault(x => x.Name.ToLower().Equals(first.RouteOfAdministration))?.RouteOfAdministrationId ?? 0,
                        PreparationInstruction = first.PreparationInstruction,
                        ApplicationInstruction = first.ApplicationInstruction,
                        SameDoseForEveryAplication = first.IsDoseSame?.ToLower()?.Equals("yes") ?? false,
                        HasMaximalCumulativeDose = first.HasMaximalCumulative?.ToLower()?.Equals("yes") ?? false,
                        CumulativeDose = first.MaximalCumulativeDose,
                        MedicationDoses = medication.Select(x => new MedicationDose()
                        {
                            DayNumber = i++,
                            UnitId = unitDAL.GetByNameId(x.Unit),
                            IntervalId = 1,
                            MedicationDoseTimes = new List<MedicationDoseTime>()
                            {
                                new MedicationDoseTime()
                                {
                                    Dose = x.Amount,
                                    Time = "9:00"
                                }
                            }

                        })
                        .ToList(),
                        CumulativeDoseUnitId = unitDAL.GetByNameId(first.Unit),
                    };

                    medications.Add(m);
                }

                List<Indication> indications = new List<Indication>();
                if (!string.IsNullOrWhiteSpace(schema.Key.IndicationAnatomicalTumorName))
                {
                    indications.Add(new Indication()
                    {
                        Name = schema.Key.IndicationAnatomicalTumorName
                    });
                }

                if (!string.IsNullOrWhiteSpace(schema.Key.IndicationMorphologicalTumorType))
                {
                    indications.Add(new Indication()
                    {
                        Name = schema.Key.IndicationMorphologicalTumorType
                    });
                }

                if (!string.IsNullOrWhiteSpace(schema.Key.IndicationTreatmentIntent))
                {
                    indications.Add(new Indication()
                    {
                        Name = schema.Key.IndicationTreatmentIntent
                    });
                }

                if (!string.IsNullOrWhiteSpace(schema.Key.IndicationTumorStage))
                {
                    indications.Add(new Indication()
                    {
                        Name = schema.Key.IndicationTumorStage
                    });
                }
                ChemotherapySchemaDB dbChema = new ChemotherapySchemaDB()
                {
                    Name = schema.Key.SchemaName,
                    Medications = medications,
                    Indications = indications,
                    CreatorId = creatorId
                };
                chemotherapySchemas.Add(dbChema);
            }

            return chemotherapySchemas;
        }
    }
}
