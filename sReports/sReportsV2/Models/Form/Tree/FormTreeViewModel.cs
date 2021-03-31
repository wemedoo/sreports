using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Form.Tree
{
    public class FormTreeViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ThesaurusId { get; set; }
        public List<FormTreeChapterViewModel> Chapters { get; set; } = new List<FormTreeChapterViewModel>();
    }
}