using sReportsV2.DTOs.DigitalGuideline.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuideline.DataIn.EvidenceProperties
{
    public class EvidencePropertiesDataIn
    {
        public List<PublicationDTO> Publications { get; set; }
        public EvidenceCategoryDataIn EvidenceCategory { get; set; }
    }
}