using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.CustomEnum.DataOut
{
    public class CustomEnumDataOut
    {
        public int Id { get; set; }
        public string OrganizationId { get; set; }
        public CustomEnumType Type { get; set; }
        public ThesaurusEntryDataOut Thesaurus { get; set; }
        public string RowVersion { get; set; }
    }
}