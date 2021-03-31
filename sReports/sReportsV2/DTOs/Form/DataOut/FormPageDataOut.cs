using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormPageDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }
        public string Description { get; set; }
        public string ThesaurusId { get; set; }
        public FormPageImageMapDataOut ImageMap { get; set; }
        public List<List<FormFieldSetDataOut>> ListOfFieldSets { get; set; } = new List<List<FormFieldSetDataOut>>();

        public FormLayoutStyleDataOut LayoutStyle { get; set; }
    }
}