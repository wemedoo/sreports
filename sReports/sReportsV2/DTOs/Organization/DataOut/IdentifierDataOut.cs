using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Organization.DataOut
{
    public class IdentifierDataOut
    {
        public IdentifierTypeDataOut System { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Use { get; set; }

        public IdentifierDataOut() { }
        public IdentifierDataOut(IdentifierTypeDataOut system, string value, string type, string use)
        {
            this.System = system;
            this.Value = value;
            this.Type = type;
            this.Use = use;
        }
    }
}