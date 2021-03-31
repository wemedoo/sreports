using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Form.Tree
{
    public class FormTreeFieldViewModel
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string ThesaurusId { get; set; }
        public List<FormTreeFieldValueViewModel> Values { get; set; } = new List<FormTreeFieldValueViewModel>();
    }
}