using sReportsV2.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormChapterDataOut
    {
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Title { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public bool IsReadonly { get; set; }
        public List<FormPageDataOut> Pages { get; set; } = new List<FormPageDataOut>();
    }
}