using sReportsV2.Common.Enums;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.ThesaurusEntry.DTO;
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

        [DataMember(Name = "state")]
        public ThesaurusState State { get; set; }

        public List<ThesaurusEntryCodingSystemDTO> CodingSystems { get; set; }

        public List<O4CodeableConceptDataIn> Codes { get; set; }

        public DateTime? LastUpdate { get; set; }


    }
}