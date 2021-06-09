using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.DTOs.ThesaurusEntry
{
    [DataContract]
    public class ThesaurusEntryTranslationDataIn
    {
        [DataMember(Name = "Id")]
        public string Id { get; set; }
        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataMember(Name = "definition")]
        [AllowHtml]
        public string Definition { get; set; }

        [DataMember(Name = "preferredTerm")]
        public string PreferredTerm { get; set; }

        [DataMember(Name = "parentId")]
        public string ParentId { get; set; }

        [DataMember(Name = "similarTerms")]
        public List<SimilarTermDTO> SimilarTerms { get; set; }

        [DataMember(Name = "synonyms")]
        public List<string> Synonyms { get; set; }

        [DataMember(Name = "abbreviations")]
        public List<string> Abbreviations { get; set; }

        public int ThesaurusEntryId { get; set; }

    }
}