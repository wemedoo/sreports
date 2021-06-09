using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormCommentListDataOut
    {
        public string Id { get; set; }
        public UserDataOut User { get; set; }
        public string FormChapterRef { get; set; }
        public string FormPageRef { get; set; }
        public string FieldSetRef { get; set; }
        public string FieldRef { get; set; } 
        public string FieldCheckRef { get; set; }
        public FormDataOut Form { get; set; }
        public FormTypeField FormTypeField { get; set; }
        public List<FormCommentDataOut> FormCommentDataOuts { get; set; } = new List<FormCommentDataOut>();

    }
}