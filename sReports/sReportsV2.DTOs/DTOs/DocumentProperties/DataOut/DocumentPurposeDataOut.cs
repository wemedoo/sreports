using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Entities.DocumentProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DocumentProperties.DataOut
{
    public class DocumentPurposeDataOut
    {
        public DocumentGeneralPurposeDataOut GeneralPurpose { get; set; }
        public DocumentExplicitPurpose? ExplicitPurpose { get; set; }
    }
}