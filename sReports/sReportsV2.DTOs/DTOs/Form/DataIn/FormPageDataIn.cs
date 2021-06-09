using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormPageDataIn
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }
        public string Description { get; set; }
        public string ThesaurusId { get; set; }
        public FormPageImageMapDataIn ImageMap { get; set; }
        public List<List<FormFieldSetDataIn>> ListOfFieldSets { get; set; } = new List<List<FormFieldSetDataIn>>();

        public FormLayoutStyleDataIn LayoutStyle { get; set; }
    }
}