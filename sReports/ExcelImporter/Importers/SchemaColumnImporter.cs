using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using sReportsV2.DTOs.DTOs.SmartOncology.Enum.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelImporter.Importers
{
    public class SchemaColumnImporter : ExcelImporter
    {
        private readonly string columnName;

        public SchemaColumnImporter(string fileName, string sheetName, string columnName) : base(fileName, sheetName)
        {
            this.columnName = columnName;
        }

        public List<SmartOncologyEnumDataOut> GetEnumsFromExcel()
        {
            List<SmartOncologyEnumDataOut> enums = new List<SmartOncologyEnumDataOut>();

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(AppDomain.CurrentDomain.BaseDirectory + $"\\App_Data\\{fileName}.xlsx", false))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                if (workbookPart != null)
                {
                    SetTextItems(workbookPart);
                    WorksheetPart worksheetPart = GetWorksheet(workbookPart, sheetName);
                    if (worksheetPart != null)
                    {
                        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                        if (sheetData != null)
                        {
                            enums = GetEnumsFromColumn(sheetData, columnName);
                        }
                    }
                }
            }

            return enums;
        }

        private List<SmartOncologyEnumDataOut> GetEnumsFromColumn(SheetData sheetData, string columnName)
        {
            List<SmartOncologyEnumDataOut> enums = new List<SmartOncologyEnumDataOut>();
            var headerCell = GetHeaderCell(sheetData, columnName);
            var cellsInColumn = GetCellsInColumn(sheetData, headerCell.CellReference);
            IEnumerable<string> textValues = cellsInColumn.Select(c => GetText(c)).Distinct();
            enums = textValues.Select(text => new SmartOncologyEnumDataOut() { Name = text, Type = columnName }).ToList();

            return enums;
        }

        private Cell GetHeaderCell(SheetData sheetData, string columnName)
        {
            return sheetData.Descendants<Row>().FirstOrDefault().Descendants<Cell>().FirstOrDefault(c => GetText(c) == columnName);
        }

        private IEnumerable<Cell> GetCellsInColumn(SheetData sheetData, string columnCellAddress)
        {
            return sheetData.Descendants<Row>().SelectMany(r => r.Descendants<Cell>()).Where(c => GetColumnName(c.CellReference) == GetColumnName(columnCellAddress)).Skip(1);
        }
    }
}
