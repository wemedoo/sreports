using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form
{
    public class FormEpisodeOfCareDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ThesaurusId { get; set; }
        public DateTime EntryDatetime { get; set; }
    }
}