using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace sReportsV2.DTOs.EmailHelper
{
    public class EmailSender
    {
        public static async void SendAsync(string subject, string plainTextContent, string htmlContent, string to)
        {
            var apiKey = WebConfigurationManager.AppSettings["apiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("jovan.prodanovic20@gmail.com", "Oncology Analytics");
            var msg = MailHelper.CreateSingleEmail(from, new EmailAddress(to), subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}