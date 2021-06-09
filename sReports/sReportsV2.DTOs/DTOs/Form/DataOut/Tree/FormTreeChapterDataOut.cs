using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut.Tree
{
    public class FormTreeChapterDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public List<FormTreePageDataOut> Pages { get; set; } = new List<FormTreePageDataOut>();
        public List<FormTreePageDataOut> SelectPages(int thesaurusId)
        {
            List<FormTreePageDataOut> pages = new List<FormTreePageDataOut>();
            List<FormTreeFieldSetDataOut> fieldSets = new List<FormTreeFieldSetDataOut>();
            foreach (FormTreePageDataOut page in this.Pages)
            {
                if (page.ThesaurusId == thesaurusId)
                    pages.Add(page);
                else
                {
                    fieldSets = page.SelectFieldSets(thesaurusId);

                    if (fieldSets.Count > 0)
                        pages.Add(page);
                }
            }
            return pages;
        }
    }
}