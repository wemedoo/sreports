using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common
{
    public class DataIn
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }
    }
}