using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut.Tree
{
    public class TreeDataOut
    {
        public int O4MtId { get; set; }

        public List<FormTreeDataOut> Forms { get; set; }
    }
}