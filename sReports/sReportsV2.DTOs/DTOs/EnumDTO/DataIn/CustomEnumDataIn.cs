using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.CustomEnum
{
    public class CustomEnumDataIn
    {
        public int Id { get; set; }
        public CustomEnumType Type { get; set; }
        public string Label { get; set; }
        public int OrganizationId { get; set; }
        public string RowVersion { get; set; }
    }
}