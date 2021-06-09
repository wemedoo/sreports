using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut.Tree
{
    public class FormTreeDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public List<FormTreeChapterDataOut> Chapters { get; set; } = new List<FormTreeChapterDataOut>();
        public int ThesaurusAppearances { get; set; }
        public List<FormTreeChapterDataOut> SelectChapters(int thesaurusId)
        {
            List<FormTreeChapterDataOut> chapters = new List<FormTreeChapterDataOut>();
            List<FormTreePageDataOut> pages = new List<FormTreePageDataOut>();
            foreach (FormTreeChapterDataOut chapter in this.Chapters) 
            {
                if (chapter.ThesaurusId == thesaurusId)
                    chapters.Add(chapter);
                else 
                {
                    pages = chapter.SelectPages(thesaurusId);

                    if (pages.Count > 0)
                        chapters.Add(chapter);
                }
            }
            return chapters;
        }
    }
}