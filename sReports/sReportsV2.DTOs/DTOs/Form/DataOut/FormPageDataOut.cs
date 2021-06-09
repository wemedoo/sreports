using sReportsV2.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormPageDataOut
    {
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Title { get; set; }
        [DataProp]
        public bool IsVisible { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public FormPageImageMapDataOut ImageMap { get; set; }
        public List<List<FormFieldSetDataOut>> ListOfFieldSets { get; set; } = new List<List<FormFieldSetDataOut>>();
        [DataProp]
        public FormLayoutStyleDataOut LayoutStyle { get; set; }
    }
}