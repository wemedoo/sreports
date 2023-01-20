using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelImporter.Classes
{
    public class RowInfo
    {
        public Dictionary<string, CellInfo> CellsInRow { get; set; } = new Dictionary<string, CellInfo>();

        public string GetCellValue(string cellName)
        {
            return CellsInRow.TryGetValue(cellName, out CellInfo cell) ? cell.GetValue() : "";
        }

        public void AddCell(string cellName, CellInfo cell)
        {
            CellsInRow[cellName] = cell;
        }

        public bool HasCells()
        {
            return CellsInRow.Count > 0;
        }
    }
}
