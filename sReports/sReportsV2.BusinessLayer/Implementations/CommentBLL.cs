using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.Domain.Sql.Entities.FormComment;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Common.Helpers.EmailSender.Interface;
using sReportsV2.Common.Constants;

namespace sReportsV2.BusinessLayer.Implementations
{

    public class CommentBLL : ICommentBLL
    {
        private readonly HttpContextBase context;
        private readonly ICommentDAL commentDAL;
        private readonly IUserDAL userDAL;
        private readonly IFormDAL formDAL;
        private readonly IEmailSender emailSender;

        public CommentBLL(ICommentDAL commentDAL, IUserDAL userDAL, IFormDAL formDAL, IEmailSender emailSender)
        {
            this.commentDAL = commentDAL;
            this.userDAL = userDAL;
            this.formDAL = formDAL;
            this.emailSender = emailSender;
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
            Comment comment = Mapper.Map<Comment>(commentDataIn);
            InsertOrUpdate(comment);
            NotifyTaggedUserInComments(commentDataIn.TaggedUsers, commentDataIn.FormRef, commentDataIn.UserId, comment.CommentId);
        }

        public string ReplayComment(string commText, int commentId, int userId, List<int> taggedUsers)
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
            NotifyTaggedUserInComments(taggedUsers, comment.FormRef, userId, reply.CommentId);

            return comment.FormRef;
        }

        public string UpdateState(int commentId, CommentState state)
        {
            var comment = commentDAL.FindById(commentId);
            commentDAL.UpdateState(commentId, state);

            return comment.FormRef;
        }

        private void NotifyTaggedUserInComments(List<int> taggedUsers, string formRef, int commentAuthorId, int commentId)
        {
            if (taggedUsers != null)
            {
                string domain = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                foreach (User taggedUser in userDAL.GetAllByIds(taggedUsers))
                {
                    sReportsV2.Domain.Entities.Form.Form form = formDAL.GetForm(formRef);
                    int thesaurusId = form.ThesaurusId;
                    string versionId = form.Version.Id;

                    string url = $"{domain}/Form/Edit?thesaurusId={thesaurusId}&versionId={versionId}&activeTab={"comments"}&taggedCommentId={commentId}";
                    
                    User commentAuthor = userDAL.GetById(commentAuthorId);
                    string title = $"User({commentAuthor.FirstName} {commentAuthor.LastName}) mentioned you in a comment on {EmailSenderNames.SoftwareName}";

                    string mailContent = $"<div>" +
                        $"Dear {taggedUser.FirstName} {taggedUser.LastName},<br><br>You have been mentioned in comment.<br>" +
                        $"Use the following link to access comment: <a href=\"{url}\">View comment</a><br>" +
                        $"If you need any help or you have any additional questions please write on the <a href='mailto:smartoncology@wemedoo.com'>smartoncology@wemedoo.com</a>.<br><br>" +
                        $"------------------------------------------------------------------------------" +
                        $"</div>";
                    Task.Run(() => emailSender.SendAsync(title, string.Empty, mailContent, taggedUser.Email));
                }
            }
        }
    }
}
