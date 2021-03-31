using sReportsV2.Domain.Entities.FieldEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    public class ReferralForm
    {
        public string Title { get; set; }
        public List<Field> Fields { get; set; }
    }
}
