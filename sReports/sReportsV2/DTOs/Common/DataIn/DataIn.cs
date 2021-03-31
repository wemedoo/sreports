using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common
{
    public class DataIn
    {
        public int Page { get; set; }
        public int PageSize { get; set; } = 5;
    }
}