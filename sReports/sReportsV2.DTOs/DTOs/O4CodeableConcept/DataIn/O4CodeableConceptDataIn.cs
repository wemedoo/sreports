using sReportsV2.DTOs.CustomAttributes;
using sReportsV2.DTOs.DTOs.CodeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.O4CodeableConcept.DataIn
{
    public class O4CodeableConceptDataIn
    {
        public int? Id { get; set; }
        public CodeSystemDataIn System { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Version { get; set; }
        public string Link { get; set; }
        [Year]
        public DateTime? VersionPublishDate { get; set; }
        public int CodeSystemId { get; set; }
        public string CodeSystemAbbr { get; set; }


    }
}