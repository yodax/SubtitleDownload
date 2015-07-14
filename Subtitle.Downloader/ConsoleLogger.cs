using System;

namespace Subtitle.Downloader
{
    public class ConsoleLogger : IMailer
    {
        public void Send(string subject, string mailBody)
        {
            Console.WriteLine(subject);
            Console.WriteLine();
            Console.WriteLine(mailBody);
            Console.WriteLine();
            Console.WriteLine("===============================");
            Console.WriteLine();
        }
    }
}