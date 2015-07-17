using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Subtitle.Provider.Addic7ed
{
    public static class FoundLinkExtensions
    {
        public static EpisodeInfo GetEpisodeInfo(this FoundLink foundLink)
        {
            // http://www.addic7ed.com/serie/Ripper_Street/2/7/Our_Betrayal_(1)
            // http://www.addic7ed.com/search.php?search=Anger+Management+2x43&Submit=Search
            if (foundLink.Link.Contains("/search.php"))
            {
                var regex = new Regex(@"search=(.+)\+(\d*)x(\d*)&Submit");
                return ExtractEpisodeInfoFromRegex(foundLink, regex);
            }
            else
            {
                var regex = new Regex(@".*serie/(.+)/(\d*)/(\d*)/");
                return ExtractEpisodeInfoFromRegex(foundLink, regex);
            }
        }

        private static EpisodeInfo ExtractEpisodeInfoFromRegex(FoundLink foundLink, Regex regex)
        {
            var match = regex.Match(foundLink.Link);

            var showName = match.Groups[1].Value.Replace('_', ' ');
            showName = showName.Replace('+', ' ');
            showName = WebUtility.UrlDecode(showName);

            var season = Convert.ToInt32(match.Groups[2].Value);
            var episode = Convert.ToInt32(match.Groups[3].Value);

            return new EpisodeInfo
            {
                ShowName = showName,
                SeasonEpisode = ExtensionMethods.GenerateSeasonEpisode(season, episode)
            };
        }
    }
}