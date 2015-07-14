namespace Subtitle.Provider.Addic7ed
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using HtmlAgilityPack;

    public static class AddictedSearchPageParser
    {
        public static string ToSearchUrl(this string showName)
        {
            return string.Format(@"http://www.addic7ed.com/search.php?search={0}&Submit=Search",
                WebUtility.UrlEncode(showName));
        }

        public static AddictedSearchPage For(string pageContent)
        {
            if (String.IsNullOrEmpty(pageContent))
                throw new EpisodePageIsEmtpyException();

            var html = new HtmlDocument();
            html.LoadHtml(pageContent);

            var htmlLinks = html.DocumentNode.SelectNodes("//table[@class='tabel']//a");
            var foundLinks = new List<FoundLink>();

            foreach (var htmlLink in htmlLinks)
            {
                if (!htmlLink.Attributes["href"].Value.Contains("serie"))
                    continue;

                var foundLink = new FoundLink
                {
                    FoundOn = DateTime.Now,
                    Link = "http://www.addic7ed.com/" + htmlLink.Attributes["href"].Value,
                    IgnoreAge = true
                };

                var episodeInfo = foundLink.GetEpisodeInfo();
                foundLink.SeasonEpisode = episodeInfo.SeasonEpisode;
                foundLink.ShowName = episodeInfo.ShowName;

                foundLinks.Add(foundLink);
            }
            return new AddictedSearchPage(foundLinks);
        }
    }
}