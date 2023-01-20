using ExcelImporter.Classes;
using ExcelImporter.Constants;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;

namespace ExcelImporter.Importers
{
    public class RouteOfAdministrationImporter : ExcelSaxImporter<RouteOfAdministration>
    {
        private readonly IRouteOfAdministrationDAL routeOfAdministrationDAL;

        public RouteOfAdministrationImporter(IRouteOfAdministrationDAL routeOfAdministrationDAL, string fileName, string sheetName) : base(fileName, sheetName)
        {
            this.routeOfAdministrationDAL = routeOfAdministrationDAL;
        }

        public override void ImportDataFromExcelToDatabase()
        {
            if (routeOfAdministrationDAL.GetAllCount() == 0)
            {
                List<RouteOfAdministration> bodySurfaceCalculationFormulaEntries = ImportFromExcel();
                InsertDataIntoDatabase(bodySurfaceCalculationFormulaEntries);
            }
        }

        protected override List<RouteOfAdministration> ImportFromExcel()
        {
            List<RowInfo> dataRows = ImportRowsFromExcel();
            List<RouteOfAdministration> routeOfAdministrations = GetRouteOfAdministrations(dataRows);
            return routeOfAdministrations;
        }

        protected override void InsertDataIntoDatabase(List<RouteOfAdministration> routeOfAdministrations)
        {
            routeOfAdministrationDAL.InsertMany(routeOfAdministrations);
        }

        private List<RouteOfAdministration> GetRouteOfAdministrations(List<RowInfo> dataRows)
        {
            List<RouteOfAdministration> routeOfAdministrations = new List<RouteOfAdministration>();

            foreach (var dataRow in dataRows)
            {
                RouteOfAdministration routeOfAdministration = GetRouteOfAdministration(dataRow);
                routeOfAdministrations.Add(routeOfAdministration);
            }

            return routeOfAdministrations;
        }

        private RouteOfAdministration GetRouteOfAdministration(RowInfo dataRow)
        {
            return new RouteOfAdministration()
            {
                Name = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaConstants.Name)),
                Definition = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaConstants.Definition)),
                ShortName = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaConstants.ShortName)),
                FDACode = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaConstants.FdaCode)),
                NCICondeptId = dataRow.GetCellValue(GetColumnAddress(ChemotherapySchemaConstants.NciConceptId))
            };
        }

    }
}
