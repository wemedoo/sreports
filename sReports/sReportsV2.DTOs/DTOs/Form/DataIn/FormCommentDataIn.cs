using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormCommentDataIn
    {
        public string Id { get; set; }
        public CommentState CommentState { get; set; }
        public string Value { get; set; }
        public string ItemRef { get; set; }
        public string CommentRef { get; set; }
        public string FormRef { get; set; }
        public int UserId { get; set; }
        public DateTime EntryDatetime { get; set; }
        public UserDataOut User { get; set; }
    }
}