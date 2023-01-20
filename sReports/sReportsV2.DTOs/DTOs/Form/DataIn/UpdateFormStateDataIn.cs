using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Form.DataIn
{
    public class UpdateFormStateDataIn
    {
        public string Id { get; set; }
        public FormDefinitionState State { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
