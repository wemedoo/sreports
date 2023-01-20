using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstanceCovidFilter
    {
        public DateTime LastUpdate { get; set; }
        public int ThesaurusId { get; set; }
        public string FieldThesaurusId { get; set; }
    }
}
