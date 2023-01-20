using ExcelImporter.Classes;
using ExcelImporter.Constants;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ExcelImporter.Importers
{
    public class MedicationDoseTypeImporter : ExcelSaxImporter<MedicationDoseType>
    {
        private readonly IMedicationDoseTypeDAL medicationDoseTypeDAL;

        public MedicationDoseTypeImporter(IMedicationDoseTypeDAL medicationDoseTypeDAL, string fileName, string sheetName) : base(fileName, sheetName)
        {
            this.medicationDoseTypeDAL = medicationDoseTypeDAL;
        }

        public override void ImportDataFromExcelToDatabase()
        {
            if (medicationDoseTypeDAL.GetAllCount() == 0)
            {
                List<MedicationDoseType> medicationDoseTypeEntries = ImportFromExcel();
                InsertDataIntoDatabase(medicationDoseTypeEntries);
            }
        }

        protected override List<MedicationDoseType> ImportFromExcel()
        {
            List<RowInfo> dataRows = ImportRowsFromExcel();
            List<MedicationDoseType> formulaEntries = GetMedicationDoseTypes(dataRows);
            return formulaEntries;
        }

        protected override void InsertDataIntoDatabase(List<MedicationDoseType> medicationDoseTypeEntries)
        {
            medicationDoseTypeDAL.InsertMany(medicationDoseTypeEntries);
        }

        private List<MedicationDoseType> GetMedicationDoseTypes(List<RowInfo> dataRows)
        {
            List<MedicationDoseType> formulas = new List<MedicationDoseType>();
            
            foreach(var dataRow in dataRows)
            {
                MedicationDoseType bodySurfaceCalculationFormula = GetMedicationDoseType(dataRow);
                formulas.Add(bodySurfaceCalculationFormula);
            }

            return formulas;
        }

        private MedicationDoseType GetMedicationDoseType(RowInfo dataRow)
        {
            List<string> intervalsList = ParseCollectionTypeFromExcel(dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaConstants.Intervals)));
            List<string> intervlasTimeFormat = intervalsList.Select(x => SetTime(x)).ToList();
            return new MedicationDoseType()
            {
                Type = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaConstants.Name)),
                IntervalsList = intervlasTimeFormat
            };
        }

        private string SetTime(string time)
        {
            if (time.Contains("."))
            {
                return time.Replace('.', ':');
            } 
            else
            {
                return time + ":00";
            }
        }

    }
}
