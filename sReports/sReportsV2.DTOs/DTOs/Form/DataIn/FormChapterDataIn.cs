using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormChapterDataIn
    {
        public string Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public string ThesaurusId { get; set; }
        public bool IsReadonly { get; set; }
        public List<FormPageDataIn> Pages { get; set; } = new List<FormPageDataIn>();
    }
}