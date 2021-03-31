using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Organization.DataIn
{
    public class IdentifierDataIn
    {
        public string System { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Use { get; set; }

        public IdentifierDataIn() { }
        public IdentifierDataIn(string system, string value, string type, string use)
        {
            this.System = system;
            this.Value = value;
            this.Type = type;
            this.Use = use;
        }
    }
}