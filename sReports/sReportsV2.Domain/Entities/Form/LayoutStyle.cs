using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    public class LayoutStyle
    {
        public LayoutType? LayoutType { get; set; }
        public string MaxItems { get; set; }
    }
}
