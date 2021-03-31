using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class ReferralInfoDTO
    {
        public string Title { get; set; }
        public List<KeyValueDTO> ReferrableFields { get; set; }
    }
}