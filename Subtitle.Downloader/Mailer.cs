namespace Subtitle.Downloader
{
    using System.Net.Mail;

    public class Mailer : IMailer
    {
        private readonly string mailFrom;
        private readonly string mailTo;
        private readonly string smtpServer;

        public Mailer(string smtpServer, string mailFrom, string mailTo)
        {
            this.mailTo = mailTo;
            this.mailFrom = mailFrom;
            this.smtpServer = smtpServer;
        }

        public void Send(string subject, string mailBody)
        {
            var message = new MailMessage(mailFrom, mailTo)
            {
                Subject = subject,
                Body = mailBody
            };
            var smtp = new SmtpClient {Host = smtpServer};

            smtp.Send(message);
        }
    }
}