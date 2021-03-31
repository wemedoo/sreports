using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Patient
{
    public class IdentifierTypeDataOut
    {
        public string O4MtId { get; set; }

        public string Name { get; set; }

        public IdentifierTypeDataOut() { }

        public IdentifierTypeDataOut(string o4MtId, string name)
        {
            this.O4MtId = o4MtId;
            this.Name = name;
        }
    }
}