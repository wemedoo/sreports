using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn.Custom.Action
{

    public class ControllerActionDataIn : CustomActionDataIn
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
    }
}