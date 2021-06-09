using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DfD.DfDDataOut
{
    public class DoctorDataOut
    {
        public string id { get; set; }
        public string invalidationToken { get; set; }
        public string gender { get; set; }
    }
}