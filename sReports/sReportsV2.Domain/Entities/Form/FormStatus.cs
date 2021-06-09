using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    public class FormStatus
    {
        public FormDefinitionState Status { get; set; }
        public DateTime Created { get; set; }
        public int UserId { get; set; }
    }
}
