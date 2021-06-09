using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.FormComment;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.Common.Enums;

namespace sReportsV2.BusinessLayer.Implementations
{
    
    public class CommentBLL : ICommentBLL
    {
        private readonly HttpContextBase context;
        private readonly ICommentDAL commentDAL;
        private readonly IUserDAL userDAL;
        private readonly IFormDAL formDAL;

        public CommentBLL(ICommentDAL commentDAL, IUserDAL userDAL, IFormDAL formDAL)
        {
            this.commentDAL = commentDAL;
            this.userDAL = userDAL;
            this.formDAL = formDAL;
        }

        public List<FormCommentDataOut> GetComentsDataOut(string formId, List<string> formItemsOrderIds)
        {
            List<Comment> comments = commentDAL.FindCommentsByFormId(formId);

            List<FormCommentDataOut> commentsDataOut = Mapper.Map<List<FormCommentDataOut>>(comments);
            List<int> userIds = comments.Select(x => x.UserId).Distinct().ToList();
            List<UserDataOut> users = Mapper.Map<List<UserDataOut>>(userDAL.GetAllByIds(userIds));
            foreach (var comment in commentsDataOut)
            {
                comment.User = users.FirstOrDefault(x => x.Id == comment.UserId);
            }
            
            commentsDataOut = commentsDataOut.OrderBy(x => formItemsOrderIds.IndexOf(x.ItemRef)).ToList();

            return commentsDataOut;
        }

        public void InsertOrUpdate(Comment comment)
        {
            commentDAL.InsertOrUpdate(comment);
        }

        public void InsertOrUpdate(FormCommentDataIn commentDataIn)
        {
            this.InsertOrUpdate(Mapper.Map<Comment>(commentDataIn));
        }

        public string ReplayComment(string commText, int commentId, int userId)
        {
            Comment comment = commentDAL.FindById(commentId);
            Comment reply = new Comment()
            {
                Value = commText,
                CommentRef = commentId,
                UserId = userId,
                FormRef = comment.FormRef,
                ItemRef = comment.ItemRef
            };

            commentDAL.InsertOrUpdate(reply);

            return comment.FormRef;
        }

        public string UpdateState(int commentId, CommentState state)
        {
            var comment = commentDAL.FindById(commentId);
            commentDAL.UpdateState(commentId, state);

            return comment.FormRef;
        }
    }
}
