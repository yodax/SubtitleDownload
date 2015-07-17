namespace Subtitle.Downloader
{
    public interface IMailer
    {
        void Send(string subject, string mailBody);
    }
}