using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.O4CodeableConcept.DataIn
{
    public class O4CodeableConceptDataIn
    {
        public string System { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Version { get; set; }
        public DateTime? VersionPublishDate { get; set; }

    }
}