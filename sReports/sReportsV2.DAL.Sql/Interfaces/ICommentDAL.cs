using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.FormComment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface ICommentDAL
    {
        Comment FindById(int id);
        void InsertOrUpdate(Comment comment);
        List<Comment> FindCommentsByFormId(string formId);
        void UpdateState(int commentId, CommentState state);
    }
}
