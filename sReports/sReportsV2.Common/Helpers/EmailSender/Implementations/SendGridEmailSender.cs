using SendGrid;
using SendGrid.Helpers.Mail;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers.EmailSender.Interface;
using System.Web.Configuration;

namespace sReportsV2.Common.Helpers.EmailSender.Implementations
{
    public class SendGridEmailSender : IEmailSender
    {
        public async void SendAsync(string subject, string plainTextContent, string htmlContent, string to)
        {
            var apiKey = WebConfigurationManager.AppSettings["AppEmailKey"];
            var email = WebConfigurationManager.AppSettings["AppEmail"];
            var sendGridClient = new SendGridClient(apiKey);
            var from = new EmailAddress(email, EmailSenderNames.SoftwareName);
            var msg = MailHelper.CreateSingleEmail(from, new EmailAddress(to), subject, plainTextContent, htmlContent);
            await sendGridClient.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}
