using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelImporter.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ExcelImporter.Importers
{
    public abstract class ExcelSaxImporter<T> : ExcelImporter
    {
        protected Dictionary<string, string> headerAddressMappings;
        protected ExcelSaxImporter(string fileName, string sheetName) : base(fileName, sheetName) {}

        public abstract void ImportDataFromExcelToDatabase();

        protected abstract List<T> ImportFromExcel();

        protected abstract void InsertDataIntoDatabase(List<T> entries);

        protected List<RowInfo> ImportRowsFromExcel()
        {
            List<RowInfo> dataRows = new List<RowInfo>();
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(AppDomain.CurrentDomain.BaseDirectory + $"\\App_Data\\{fileName}.xlsx", false))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                if (workbookPart != null)
                {
                    SetTextItems(workbookPart);
                    WorksheetPart worksheetPart = GetWorksheet(workbookPart, sheetName);
                    if (worksheetPart != null)
                    {
                        var parsedRows = ParseRows(worksheetPart);
                        SetHeaderRowInfo(parsedRows.FirstOrDefault());
                        dataRows = parsedRows.Count > 1 ? parsedRows.Skip(1).ToList() : new List<RowInfo>();
                    }
                }
            }
            return dataRows;
        }

        protected string GetColumnAddress(string key)
        {
            return headerAddressMappings.TryGetValue(key, out string address) ? address : "";
        }

        protected List<string> ParseCollectionTypeFromExcel(string excelValue)
        {
            return string.IsNullOrWhiteSpace(excelValue) ? new List<string>() : excelValue.Split('|').ToList();
        }

        private List<RowInfo> ParseRows(WorksheetPart worksheetPart)
        {
            List<RowInfo> dataRows = new List<RowInfo>();

            RowInfo rowInfo = null;
            CellInfo cellInfo = null;

            OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
            while (reader.Read())
            {
                bool startElement = reader.IsStartElement;
                bool endElement = reader.IsEndElement;
                bool hasAttr = reader.HasAttributes;
                var attrs = reader.Attributes;

                if (IsStartOfElement(startElement, typeof(Row), reader))
                {
                    rowInfo = new RowInfo();
                }
                if (IsStartOfElement(startElement, typeof(Cell), reader))
                {
                    cellInfo = new CellInfo();
                    ReadCell(hasAttr, attrs, cellInfo);
                }
                if (IsStartOfElement(startElement, typeof(CellValue), reader))
                {
                    ReadCellValue(reader.GetText(), cellInfo);
                }
                if (IsEndOfElement(endElement, typeof(Cell), reader))
                {
                    AddCell(cellInfo, rowInfo);
                    cellInfo = null;
                }
                if (IsEndOfElement(endElement, typeof(Row), reader))
                {
                    AddRow(rowInfo, dataRows);
                    rowInfo = null;
                }
            }

            return dataRows;
        }

        private void SetHeaderRowInfo(RowInfo firstRow)
        {
            if (firstRow != null)
            {
                this.headerAddressMappings = firstRow.CellsInRow.ToDictionary(kv => kv.Value.Value, kv => kv.Key);
            }
        }

        private void ReadCell(bool hasAttr, ReadOnlyCollection<OpenXmlAttribute> attributes, CellInfo cell)
        {
            if (hasAttr)
            {
                var addressAttr = attributes.FirstOrDefault(a => a.LocalName == "r");
                var dataTypeAttr = attributes.FirstOrDefault(a => a.LocalName == "t");
                cell.Address = addressAttr.Value;
                cell.DataType = dataTypeAttr.Value;
            }
        }

        private void ReadCellValue(string text, CellInfo cell)
        {
            if (cell.DataType == "s")
            {
                cell.Value = GetStringValue(text);
            }
            else
            {
                cell.Value = text;
            }
        }

        private void AddCell(CellInfo cell, RowInfo row)
        {
            row.AddCell(GetColumnName(cell.Address), cell);
        }

        private void AddRow(RowInfo row, List<RowInfo> rows)
        {
            if (row.HasCells())
            {
                rows.Add(row);
            }
        }

        private bool IsStartOfElement(bool startOfElement, Type elementType, OpenXmlReader reader)
        {
            return startOfElement && reader.ElementType == elementType;
        }

        private bool IsEndOfElement(bool isEndElement, Type elementType, OpenXmlReader reader)
        {
            return isEndElement && reader.ElementType == elementType;
        }
    }
}
