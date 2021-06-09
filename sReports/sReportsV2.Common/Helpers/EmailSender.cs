using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace sReportsV2.Common.Helpers
{
    public static class EmailSender
    {
        public static async void SendAsync(string subject, string plainTextContent, string htmlContent, string to)
        {
            var apiKey = WebConfigurationManager.AppSettings["apiKey"];
            //var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("djokic.veljko55@gmail.com", "Oncology Analytics");
            var msg = MailHelper.CreateSingleEmail(from, new EmailAddress(to), subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}