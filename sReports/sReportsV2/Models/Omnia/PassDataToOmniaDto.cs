using System.Collections.Generic;

namespace sReportsV2.Models.Omnia
{
    public class PassDataToOmniaDto
    {
        public List<OmniaFieldValue> OmniaFieldValues { get; set; } = new List<OmniaFieldValue>();
    }
}