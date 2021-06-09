using sReportsV2.DTOs.DigitalGuideline.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuideline.DataOut.EvidenceProperties
{
    public class EvidencePropertiesDataOut
    {
        public List<PublicationDTO> Publications { get; set; }
        public EvidenceCategoryDataOut EvidenceCategory { get; set; } 
    }
}