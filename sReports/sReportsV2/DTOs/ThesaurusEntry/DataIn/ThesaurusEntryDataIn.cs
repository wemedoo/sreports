using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.DTOs.ThesaurusEntry
{
    [DataContract]
    public class ThesaurusEntryDataIn
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "o40MtId")]
        public string O40MTId { get; set; }

        [DataMember(Name = "umlsCode")]
        public string UmlsCode { get; set; }

        [DataMember(Name = "umlsName")]
        public string UmlsName { get; set; }

        [DataMember(Name = "umlsDefinitions")]
        [AllowHtml]
        public string UmlsDefinitions { get; set; }

        [DataMember(Name = "parentId")]
        public string ParentId { get; set; }

        [DataMember(Name = "translations")]
        public List<ThesaurusEntryTranslationDataIn> Translations { get; set; }

        public List<ThesaurusEntryCodingSystemDataIn> CodingSystems { get; set; }
        public List<O4CodeableConceptDataIn> Codes { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}