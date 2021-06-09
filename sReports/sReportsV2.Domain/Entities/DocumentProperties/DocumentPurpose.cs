using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DocumentProperties
{
    public class DocumentPurpose
    {
        public DocumentGeneralPurpose GeneralPurpose { get; set; }
        public DocumentExplicitPurpose? ExplicitPurpose { get; set; }
    }
}
