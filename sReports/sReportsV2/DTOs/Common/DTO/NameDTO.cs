using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class NameDTO
    {
        public string Given { get; set; }
        public string Family { get; set; }

        public NameDTO(string given, string family)
        {
            this.Given = given;
            this.Family = family;
        }
        public NameDTO() { }
    }
}
