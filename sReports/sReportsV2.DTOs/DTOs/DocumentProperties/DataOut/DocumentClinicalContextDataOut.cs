using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Entities.DocumentProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DocumentProperties.DataOut
{
    public class DocumentClinicalContextDataOut
    {
        public DocumentClinicalContextEnum ClinicalContext { get; set; }
        public FollowUp FollowUp { get; set; }
    }
}