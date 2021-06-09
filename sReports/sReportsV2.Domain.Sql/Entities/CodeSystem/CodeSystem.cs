using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.CodeSystem
{
    public class CodeSystem
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string SAB { get; set; }
    }
}
