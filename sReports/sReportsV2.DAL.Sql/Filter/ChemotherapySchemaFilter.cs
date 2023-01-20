using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Filter
{
    public class ChemotherapySchemaFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Indication { get; set; }
        public string State { get; set; }
        public string ClinicalConstelation { get; set; }
        public string Name { get; set; }
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }
    }
}
