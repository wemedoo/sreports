using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelImporter.Classes;
using ExcelImporter.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExcelImporter.Importers
{
    public abstract class ExcelImporter
    {
        protected readonly string fileName;
        protected readonly string sheetName;
        protected SharedStringItem[] textItems;
        protected SheetData sheetData;
        protected List<CellInfo> schemaHeaderCells;
        protected List<string> columnsToParse;
        protected ExcelImporter(string fileName, string sheetName)
        {
            this.fileName = fileName;
            this.sheetName = sheetName;
        }

        protected void SetTextItems(WorkbookPart workbookPart)
        {
            SharedStringTablePart shareStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
            textItems = shareStringPart.SharedStringTable.Elements<SharedStringItem>().ToArray();
        }

        protected WorksheetPart GetWorksheet(WorkbookPart workbookPart, string workSheetName)
        {
            IEnumerable<Sheet> sheets = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == workSheetName);
            if (sheets.Count() == 0)
            {
                return null;
            }

            return (WorksheetPart)workbookPart.GetPartById(sheets.First().Id);
        }

        protected List<CellInfo> GetColumnHeaderCellsInfo()
        {
            var cellsInHeaderRow = sheetData.Descendants<Row>().FirstOrDefault().Descendants<Cell>();
            return cellsInHeaderRow.Select(c => new CellInfo() { Name = GetText(c), Address = c.CellReference }).ToList();
        }

        protected string GetText(Cell cell)
        {
            if (cell?.CellValue != null)
            {
                return cell.DataType.Value == CellValues.SharedString ? GetStringValue(cell.CellValue.Text) : cell.CellValue.Text;
            }
            return string.Empty;
        }

        protected Cell GetLastCellInColumn(string columnCellAddress, string schemaName)
        {
            return sheetData.Descendants<Row>().SelectMany(r => r.Descendants<Cell>()).Where(c => GetColumnName(c.CellReference) == GetColumnName(columnCellAddress) && GetText(c) == schemaName).Last();
        }

        protected string GetColumnName(string cellAddress)
        {
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellAddress);

            return match.Value;
        }

        protected int GetRowIndex(string cellAddress)
        {
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(cellAddress);

            return int.Parse(match.Value);
        }

        protected int GetRowIndexOfLastCellInColumn(string columnName)
        {
            string columnAdress = GetColumnAdress(columnName);
            var lastCellInColumn = GetLastCellInColumn(columnAdress);
            return GetRowIndex(lastCellInColumn.CellReference);
        }

        protected CellInfo SetCellInfo(int rowIndex, string columnName)
        {
            var columnIndex = GetColumn(columnName);
            var cell = GetCell(rowIndex, columnIndex);
            return ParseCell(GetText(cell), columnName, string.Concat(columnIndex, rowIndex), rowIndex);
        }

        protected Cell GetLastCellInColumn(string columnCellAddress)
        {
            return sheetData.Descendants<Row>().SelectMany(r => r.Descendants<Cell>()).Where(c => GetColumnName(c.CellReference) == GetColumnName(columnCellAddress)).Last();
        }

        protected Cell GetCell(int rowIndex, string columnIndex)
        {
            return sheetData.Descendants<Row>().FirstOrDefault(p => p.RowIndex == rowIndex).Descendants<Cell>().FirstOrDefault(p => GetColumnName(p.CellReference) == columnIndex);
        }

        protected string GetColumnAdress(string columnName)
        {
            return schemaHeaderCells.FirstOrDefault(c => c.Name == columnName).Address;
        }

        protected int ParseNumericValue(string cellValue)
        {
            _ = int.TryParse(cellValue, out int number);
            return number;
        }

        private CellInfo ParseCell(string text, string columnName, string address, int rowIndex)
        {
            return new CellInfo()
            {
                Name = columnName,
                Value = text,
                Address = address,
                Row = rowIndex
            };
        }

        private string GetColumn(string columnName)
        {
            string columnAddress = GetColumnAdress(columnName);
            return GetColumnName(columnAddress);
        }

        protected string GetStringValue(string cellText)
        {
            if (string.IsNullOrEmpty(cellText)) return string.Empty;

            string stringValue = textItems[int.Parse(cellText)].InnerText;
            return stringValue == GlobalThesaurusConstants.NA ? string.Empty : stringValue;
        }

        protected List<RowInfo> ParseRows(IEnumerable<CellInfo> schemaCells)
        {
            List<RowInfo> dataRows = new List<RowInfo>();
            foreach (var schemaCell in schemaCells)
            {
                RowInfo dataRow = new RowInfo();
                foreach (string columnName in columnsToParse)
                {
                    dataRow.AddCell(columnName, SetCellInfo(schemaCell.Row, columnName));

                }
                dataRows.Add(dataRow);
            }

            return dataRows;
        }
    }
}
