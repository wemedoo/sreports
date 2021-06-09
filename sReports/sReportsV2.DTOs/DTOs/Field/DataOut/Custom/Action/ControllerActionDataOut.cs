using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut.Custom.Action
{
    public class ControllerActionDataOut : CustomActionDataOut
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
    }
}