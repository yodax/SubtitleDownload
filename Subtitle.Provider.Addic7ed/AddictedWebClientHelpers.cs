namespace Subtitle.Provider.Addic7ed
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    public static class AddictedWebClientHelpers
    {
        public static bool IsLoggedIn(this string result)
        {
            return result.Contains("logout.php");
        }

        public static string CreateLoginPostData(string userName, string password)
        {
            var valuesToSendInPost = new Dictionary<string, string>
            {
                {"username", userName},
                {"password", password},
                {"Submit", "Log in"}
            };
            var postdata = String.Join("&", valuesToSendInPost.Select(x => x.Key + "=" + WebUtility.UrlEncode(x.Value)));
            return postdata;
        }

        public static string ReadResponse(this Stream readStream)
        {
            var reader = new StreamReader(readStream);
            return reader.ReadToEnd();
        }
    }
}