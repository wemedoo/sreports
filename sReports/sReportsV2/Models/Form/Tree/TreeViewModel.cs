using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Form.Tree
{
    public class TreeViewModel
    {
        public string O4MtId { get; set; }

        public List<FormTreeViewModel> Forms { get; set; }
    }
}