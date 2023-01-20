namespace sReportsV2.Common.Helpers.EmailSender.Interface
{
    public interface IEmailSender
    {
        void SendAsync(string subject, string plainTextContent, string htmlContent, string to);
    }
}
