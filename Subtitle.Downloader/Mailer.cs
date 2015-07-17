using System.Net.Mail;

namespace Subtitle.Downloader
{
    public class Mailer : IMailer
    {
        private readonly string _mailFrom;
        private readonly string _mailTo;
        private readonly string _smtpServer;

        public Mailer(string smtpServer, string mailFrom, string mailTo)
        {
            _mailTo = mailTo;
            _mailFrom = mailFrom;
            _smtpServer = smtpServer;
        }

        public void Send(string subject, string mailBody)
        {
            var message = new MailMessage(_mailFrom, _mailTo)
            {
                Subject = subject,
                Body = mailBody
            };
            var smtp = new SmtpClient {Host = _smtpServer};

            smtp.Send(message);
        }
    }
}