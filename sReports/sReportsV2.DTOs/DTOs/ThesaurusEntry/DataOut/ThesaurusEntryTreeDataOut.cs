using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.ThesaurusEntry
{
    [DataContract]
    public class ThesaurusEntryTreeDataOut
    {

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "o40MtId")]
        public string O40MTId { get; set; }

        [DataMember(Name = "umlsCode")]
        public string UMLSCode { get; set; }

        [DataMember(Name = "definition")]
        public string Definition { get; set; }

        [DataMember(Name = "preferredTerm")]
        public string PreferredTerm { get; set; }

        [DataMember(Name = "parent")]
        public ThesaurusEntryTreeDataOut Parent { get; set; }
    }
}