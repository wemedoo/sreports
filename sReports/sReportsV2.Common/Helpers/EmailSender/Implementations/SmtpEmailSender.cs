using Serilog;
using sReportsV2.Common.Helpers.EmailSender.Interface;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;

namespace sReportsV2.Common.Helpers.EmailSender.Implementations
{
    public class SmtpEmailSender : IEmailSender
    {
        public async void SendAsync(string subject, string plainTextContent, string htmlContent, string to)
        {
            string smtpServerEmail = WebConfigurationManager.AppSettings["SmtpServerEmail"];
            string smtpServerPassword = WebConfigurationManager.AppSettings["SmtpServerPassword"];
            string smtpServerEmailDisplayName = WebConfigurationManager.AppSettings["SmtpServerEmailDisplayName"];
            string smtpServerHost = WebConfigurationManager.AppSettings["SmtpServerHost"];
            int.TryParse(WebConfigurationManager.AppSettings["SmtpServerPort"], out int smtpServerPort);
            smtpServerPort = smtpServerPort > 0 ? smtpServerPort : 22;
            bool.TryParse(WebConfigurationManager.AppSettings["SmtpServerEnableSsl"], out bool enableSsl);

            SmtpClient smtpClient = new SmtpClient
            {
                Port = smtpServerPort,
                Host = smtpServerHost,
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpServerEmail, smtpServerPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            MailMessage message = new MailMessage
            {
                From = new MailAddress(smtpServerEmail, smtpServerEmailDisplayName),
                Subject = subject,
                IsBodyHtml = true,
                Body = htmlContent
            };
            message.To.Add(new MailAddress(to));

            try
            {
                await smtpClient.SendMailAsync(message);
            }
            catch(Exception e)
            {
                LogHelper.Error($"Sending email ended up with error: ({e.Message})");
            }
            finally
            {
                message.Dispose();
            }            
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                LogHelper.Error("Send canceled.");
            }
            if (e.Error != null)
            {
                LogHelper.Error($"Sending email ended up with error: ({e.Error})");
            }
        }
    }
}
