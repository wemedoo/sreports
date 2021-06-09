using sReportsV2.DTOs.CustomEnum.DataOut;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Organization.DataOut
{
    public class IdentifierDataOut
    {
        public int Id { get; set; }
        public CustomEnumDataOut System { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Use { get; set; }
    }
}