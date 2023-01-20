using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelImporter.Classes;
using sReportsV2.DTOs.DTOs.SmartOncology.Enum.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelImporter.Importers
{
    /*
     Use to parse schemas from the excel. The schemas will be shown only in progress note tab. 
     This was implemented before we implemented UI for schema definition and its main purpose was to simulate existing schemas in the system.
     */
    public class SchemaImporter : ExcelImporter
    { 
        public SchemaImporter(string fileName, string sheetName) : base(fileName, sheetName)
        {
            columnsToParse = new List<string>()
            {
                SmartOncologyEnumNames.ChemotherapyType,
                SmartOncologyEnumNames.DayNumber,
                SmartOncologyEnumNames.MedicationName,
                SmartOncologyEnumNames.TreatmentType,
                SmartOncologyEnumNames.Dose,
                SmartOncologyEnumNames.PreparationInstruction,
                SmartOncologyEnumNames.ApplicationInstruction,
                SmartOncologyEnumNames.Notes
            };
        }

        public SchemaDataOut ImportFromExcel(string schemaName)
        {
            SchemaDataOut schema = new SchemaDataOut();

            if (!string.IsNullOrEmpty(schemaName))
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(AppDomain.CurrentDomain.BaseDirectory + $"\\App_Data\\{fileName}.xlsx", false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    if (workbookPart != null)
                    {
                        SetTextItems(workbookPart);
                        WorksheetPart worksheetPart = GetWorksheet(workbookPart, sheetName);
                        if (worksheetPart != null)
                        {
                            sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                            if (sheetData != null)
                            {
                                this.schemaHeaderCells = GetColumnHeaderCellsInfo();
                                var schemaRows = GetSchemaRowIndexes(schemaName);
                                var dataRows = ParseRows(schemaRows);
                                schema = GetSchemaData(dataRows);
                            }
                        }
                    }
                }
            }

            schema.Name = schemaName;
            schema.FirstDay = DateTime.Now;
            return schema;
        }

        private List<CellInfo> GetSchemaRowIndexes(string schemaName)
        {
            List<CellInfo> schemaCells = new List<CellInfo>();
            string schemaNameColumnAddress = GetColumnAdress(SmartOncologyEnumNames.ChemotherapyType);

            var lastCellInColumn = GetLastCellInColumn(schemaNameColumnAddress, schemaName);
            int endRow = GetRowIndex(lastCellInColumn.CellReference);

            for (int i = 1; i < endRow; i++)
            {
                var sheetRow = sheetData.Descendants<Row>().ElementAt(i);
                var schemaCell = sheetRow.Descendants<Cell>().FirstOrDefault(c => GetColumnName(c.CellReference) == GetColumnName(schemaNameColumnAddress) && GetText(c) == schemaName);
                if (schemaCell != null)
                {
                    schemaCells.Add(new CellInfo() { Row = GetRowIndex(schemaCell.CellReference), Address = schemaCell.CellReference });
                }
            }

            return schemaCells;
        }

        private SchemaDataOut GetSchemaData(List<RowInfo> dataRows)
        {
            SchemaDataOut schema = new SchemaDataOut();

            var groupByDays = dataRows.GroupBy(row => row.GetCellValue(SmartOncologyEnumNames.DayNumber));

            foreach (var groupByDay in groupByDays)
            {
                var schemaDay = GetSchemaDay(groupByDay);
                schema.AddDay(schemaDay);
            }
            schema.Days = schema.Days.OrderBy(x => x.DayNumber).ToList();

            return schema;
        }

        private SchemaDayDataOut GetSchemaDay(IGrouping<string, RowInfo> groupByDay)
        {
            SchemaDayDataOut schemaDay = new SchemaDayDataOut();
            foreach (var dataRow in groupByDay)
            {
                schemaDay.DayNumber = ParseNumericValue(dataRow.GetCellValue(SmartOncologyEnumNames.DayNumber));
                SetTreatmentData(schemaDay, dataRow);
            }

            return schemaDay;
        }

        private void SetTreatmentData(SchemaDayDataOut schemaDay, RowInfo dataRow)
        {
            SchemaMedicationDataOut medicationDataOut = GetMedicationData(dataRow);
            string treatmentType = dataRow.GetCellValue(SmartOncologyEnumNames.TreatmentType);

            switch (treatmentType)
            {
                case SmartOncologyEnumNames.CancerDirectedTreatment:
                    schemaDay.AddMedication(medicationDataOut, schemaDay.CancerDirectedTreatment);
                    break;
                case SmartOncologyEnumNames.SupportiveTherapy:
                    schemaDay.AddMedication(medicationDataOut, schemaDay.SupportiveTherapy);
                    break;
                default:
                    schemaDay.AddMedication(medicationDataOut, schemaDay.CancerDirectedTreatment);
                    schemaDay.AddMedication(medicationDataOut, schemaDay.SupportiveTherapy);
                    break;
            }
        }

        private SchemaMedicationDataOut GetMedicationData(RowInfo dataRow)
        {
            SchemaMedicationDataOut medicationData = new SchemaMedicationDataOut
            {
                Name = dataRow.GetCellValue(SmartOncologyEnumNames.MedicationName),
                Dose = dataRow.GetCellValue(SmartOncologyEnumNames.Dose),
                PreparationInstruction = dataRow.GetCellValue(SmartOncologyEnumNames.PreparationInstruction),
                ApplicationInstruction = dataRow.GetCellValue(SmartOncologyEnumNames.ApplicationInstruction),
                Notes = dataRow.GetCellValue(SmartOncologyEnumNames.Notes)
            };

            return medicationData;
        }
    }
}
