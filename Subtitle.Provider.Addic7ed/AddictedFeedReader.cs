using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Subtitle.Provider.Addic7ed
{
    public class AddictedFeedReader
    {
        public static IEnumerable<string> GetAllLinksFrom(string feedContent)
        {
            if (feedContent.Contains("feedburner"))
            {
                var regex = new Regex(@"<feedburner:origLink>(.*)</feedburner:origLink>");
                var linkMatches = regex.Matches(feedContent);

                foreach (Match linkMatch in linkMatches)
                {
                    yield return linkMatch.Groups[1].ToString();
                }
            }
            else
            {
                var regex = new Regex(@"<link>(.*)</link>");
                var linkMatches = regex.Matches(feedContent);

                for (var i = 1; i < linkMatches.Count; i++)
                {
                    yield return linkMatches[i].Groups[1].ToString();
                }
            }
        }
    }
}