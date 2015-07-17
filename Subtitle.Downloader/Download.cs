using System;
using System.Text;
using System.Threading;
using Subtitle.Common;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader
{
    public class Download : IDownload
    {
        private readonly AddictedWebClient _webClient;

        public Download(string userName, string password)
        {
            _webClient = new AddictedWebClient();

            if (!string.IsNullOrEmpty(userName))
            {
                if (!_webClient.Login(userName, password))
                {
                    throw new Exception("Can not log in!");
                }
            }
        }

        public string From(string url, string referer = null)
        {
            if (referer != null)
            {
                _webClient.Headers.Add("Referer", referer);
            }

            _webClient.Encoding = Encoding.UTF8;

            // Sleep between 1 and 2,5 seconds before downloading from the url
            Thread.Sleep(new Random().Next(1000, 2500));

            return _webClient.DownloadString(url);
        }
    }
}