using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.FormComment;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class CommentDAL : ICommentDAL
    {
        private SReportsContext context;
        public CommentDAL(SReportsContext context)
        {
            this.context = context;
        }
        public Comment FindById(int id)
        {
            return context.Comments.FirstOrDefault(c => c.Id.Equals(id));
        }

        public List<Comment> FindCommentsByFormId(string formId)
        {
            return context.Comments.OrderByDescending(x => x.EntryDatetime)
               .Where(c => c.FormRef.Equals(formId))
               .ToList();
        }

        public void InsertOrUpdate(Comment comment)
        {
            if (comment.Id == 0)
            {
                comment.EntryDatetime = DateTime.Now;
                comment.LastUpdate = DateTime.Now;
                context.Comments.Add(comment);
                context.SaveChanges();
            }
            else 
            {
                
            }
        }

        public void UpdateState(int commentId, CommentState state)
        {
            Comment comment = context.Comments.FirstOrDefault(x => x.Id == commentId);
            if (comment != null) 
            {
                comment.CommentState = state;
                comment.LastUpdate = DateTime.Now;
                context.SaveChanges();
            }
        }
    }
}
