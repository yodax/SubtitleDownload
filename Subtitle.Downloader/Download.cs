namespace Subtitle.Downloader
{
    using System;
    using System.Text;
    using System.Threading;
    using Common;
    using Provider.Addic7ed;

    public class Download : IDownload
    {
        private readonly AddictedWebClient webClient;

        public Download(string userName, string password)
        {
            webClient = new AddictedWebClient();

            if (!string.IsNullOrEmpty(userName))
            {
                if (!webClient.Login(userName, password))
                {
                    throw new Exception("Can not log in!");
                }
            }
        }

        public string From(string url, string referer = null)
        {
            if (referer != null)
            {
                webClient.Headers.Add("Referer", referer);
            }

            webClient.Encoding = Encoding.UTF8;
            
            // Sleep between 1 and 2,5 seconds before downloading from the url
            Thread.Sleep(new Random().Next(1000, 2500));
            
            return webClient.DownloadString(url);
        }
    }
}