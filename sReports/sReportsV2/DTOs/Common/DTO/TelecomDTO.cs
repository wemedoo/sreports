using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common
{
    public class TelecomDTO
    {
        public string System { get; set; }
        public string Value { get; set; }
        public string Use { get; set; }

        public TelecomDTO() { }
        public TelecomDTO(string system, string value, string use)
        {
            this.System = system;
            this.Value = value;
            this.Use = use;
        }
    }
}