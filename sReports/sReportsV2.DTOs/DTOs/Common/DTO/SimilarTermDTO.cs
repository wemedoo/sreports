using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class SimilarTermDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public SimilarTermType Source { get; set; }
        public DateTime? EntryDateTime { get; set; }
    }
}