using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelImporter.Classes
{
    public class CellInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
        public int Row { get; set; }
        public string Address { get; set; }
        public string GetValue()
        {
            return string.IsNullOrEmpty(Value) ? string.Empty : Value.Trim();
        }
    }
}
