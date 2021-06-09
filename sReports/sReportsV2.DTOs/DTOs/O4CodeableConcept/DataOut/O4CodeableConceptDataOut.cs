using sReportsV2.DTOs.CodeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.O4CodeableConcept.DataOut
{
    public class O4CodeableConceptDataOut
    {
        public int Id { get; set; }
        public CodeSystemDataOut System { get; set; }
        public string Code { get; set; }
        public string Version { get; set; }
        public string Value { get; set; }
        public string Link { get; set; }
        public DateTime? VersionPublishDate { get; set; }
        public DateTime? EntryDateTime { get; set; }

        public O4CodeableConceptDataOut() { }
       
    }
}