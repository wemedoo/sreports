using ExcelImporter.Classes;
using ExcelImporter.Constants;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;

namespace ExcelImporter.Importers
{
    public class BodySurfaceCalculationFormulaImporter : ExcelSaxImporter<BodySurfaceCalculationFormula>
    {
        private readonly IBodySurfaceCalculationFormulaDAL bodySurfaceCalculationFormulaDAL;

        public BodySurfaceCalculationFormulaImporter(IBodySurfaceCalculationFormulaDAL bodySurfaceCalculationFormulaDAL, string fileName, string sheetName) : base(fileName, sheetName)
        {
            this.bodySurfaceCalculationFormulaDAL = bodySurfaceCalculationFormulaDAL;
        }

        public override void ImportDataFromExcelToDatabase()
        {
            if (bodySurfaceCalculationFormulaDAL.GetAllCount() == 0)
            {
                List<BodySurfaceCalculationFormula> bodySurfaceCalculationFormulaEntries = ImportFromExcel();
                InsertDataIntoDatabase(bodySurfaceCalculationFormulaEntries);
            }
        }

        protected override List<BodySurfaceCalculationFormula> ImportFromExcel()
        {
            List<RowInfo> dataRows = ImportRowsFromExcel();
            List<BodySurfaceCalculationFormula> formulaEntries = GetFormulas(dataRows);
            return formulaEntries;
        }

        protected override void InsertDataIntoDatabase(List<BodySurfaceCalculationFormula> bodySurfaceCalculationFormulaEntries)
        {
            bodySurfaceCalculationFormulaDAL.InsertMany(bodySurfaceCalculationFormulaEntries);
        }

        private List<BodySurfaceCalculationFormula> GetFormulas(List<RowInfo> dataRows)
        {
            List<BodySurfaceCalculationFormula> formulas = new List<BodySurfaceCalculationFormula>();
            
            foreach(var dataRow in dataRows)
            {
                BodySurfaceCalculationFormula bodySurfaceCalculationFormula = GetFormula(dataRow);
                formulas.Add(bodySurfaceCalculationFormula);
            }

            return formulas;
        }

        private BodySurfaceCalculationFormula GetFormula(RowInfo dataRow)
        {
            return new BodySurfaceCalculationFormula()
            {
                Name = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaConstants.Name)),
                Formula = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaConstants.Formula))
            };
        }

    }
}
