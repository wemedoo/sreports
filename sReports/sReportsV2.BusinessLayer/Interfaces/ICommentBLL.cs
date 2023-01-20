using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.FormComment;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ICommentBLL
    {
        List<FormCommentDataOut> GetComentsDataOut(string formId, List<string> formListIdsOrders);
        void InsertOrUpdate(Comment comment);
        void InsertOrUpdate(FormCommentDataIn commentDataIn);

        string UpdateState(int commentId, CommentState state);
        string ReplayComment(string commText, int commentId, int userId, List<int> taggedUsers);


    }
}
