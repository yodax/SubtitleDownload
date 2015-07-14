namespace Subtitle.Provider.Addic7ed
{
    using System;
    using System.Net;
    using System.Text;

    public class AddictedWebClient : WebClient
    {
        public AddictedWebClient()
        {
            CookieContainer = new CookieContainer();
        }

        public CookieContainer CookieContainer { get; private set; }

        public bool Login(string userName, string password)
        {
            const string loginPageAddress = @"http://www.addic7ed.com/dologin.php";
            var postdata = AddictedWebClientHelpers.CreateLoginPostData(userName, password);

            var request = CreateLoginRequest(loginPageAddress, postdata);

            using (var response = request.GetResponse())
            {
                var result = response.GetResponseStream().ReadResponse();

                return result.IsLoggedIn();
            }
        }

        private HttpWebRequest CreateLoginRequest(string loginPageAddress, string postdata)
        {
            var request = (HttpWebRequest) WebRequest.Create(loginPageAddress);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var buffer = Encoding.ASCII.GetBytes(postdata);
            request.ContentLength = buffer.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            request.CookieContainer = CookieContainer;
            return request;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest) base.GetWebRequest(address);
            if (request != null)
            {
                request.CookieContainer = CookieContainer;
                request.UserAgent = @"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9) AppleWebKit/537.71 (KHTML, like Gecko) Version/7.0 Safari/537.71";
                return request;
            }

            return null;
        }
    }
}