using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstancePerDomain
    {
        public DocumentClinicalDomain? Domain { get; set; }
        public int Count { get; set; }
    }
}
