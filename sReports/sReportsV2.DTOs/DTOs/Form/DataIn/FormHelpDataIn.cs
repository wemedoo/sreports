using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormHelpDataIn
    {
        public string Title { get; set; }

        [AllowHtml]
        public string Content { get; set; }
    }
}