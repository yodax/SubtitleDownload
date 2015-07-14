namespace Subtitle.Provider.Addic7ed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;

    public class LinkFinder
    {
        private readonly IDownload download;
        private readonly List<FoundLink> foundLinks;

        public LinkFinder(List<FoundLink> foundLinks, IDownload download)
        {
            this.download = download;
            this.foundLinks = foundLinks;
        }

        public void LookForLinksFromFeeds(IEnumerable<string> feedLinks)
        {
            var linksFoundFromFeeds = new List<FoundLink>();
            foreach (var feedLink in feedLinks)
            {
                linksFoundFromFeeds.AddRange(AddictedFeedReader
                    .GetAllLinksFrom(download.From(feedLink))
                    .Where(l => l.Contains("/serie/"))
                    .Select(s => new FoundLink
                    {
                        Link = s,
                        FoundOn = DateTime.Now
                    }));
            }

            AddFoundLinksToStore(linksFoundFromFeeds);
        }

        private void AddFoundLinksToStore(IEnumerable<FoundLink> newlyFoundLinks)
        {
            var allLinks = foundLinks.Union(newlyFoundLinks).ToList();
            foundLinks.Clear();
            foundLinks.AddRange(allLinks
                .GroupBy(l => l.Link.StripAddictedEpisode())
                .Select(grp => grp.OrderByDescending(x => x.IgnoreAge).ThenBy(x => x.FoundOn)
                    .First()));

            var badLinks = new List<FoundLink>();

            foundLinks.ForEach(l =>
            {
                try
                {
                    var epinfo = l.GetEpisodeInfo();
                    l.ShowName = epinfo.ShowName;
                    l.SeasonEpisode = epinfo.SeasonEpisode;
                }
                catch
                {
                    badLinks.Add(l);
                }
            });

            badLinks.ForEach(badLink => foundLinks.Remove(badLink));
        }

        public void LookForLinksFromShow(string showName)
        {
            var links = SearchForLinksForShow(showName.ToSearchUrl());
            AddFoundLinksToStore(links);
        }

        private IEnumerable<FoundLink> SearchForLinksForShow(string searchResultUrl)
        {
            var searchUrl = searchResultUrl;
            var pageContent = download.From(searchUrl);

            try
            {
                var links = AddictedSearchPageParser.For(pageContent)
                        .FoundLinks
                        .Where(l => l.Link.Contains("/serie/"));
                return links;
            }
            catch
            {
                var subtitlePage = AddictedEpisodePageParser.For(pageContent);
                return new List<FoundLink>
                {
                    new FoundLink
                    {
                        FoundOn = DateTime.Now,
                        IgnoreAge = true,
                        Link = searchResultUrl,
                        SeasonEpisode = subtitlePage.SeasonEpisode,
                        ShowName = subtitlePage.ShowName
                    }
                };
            }
        }

        public void LookForLinksForEpisode(string showName, string episode)
        {
            var epIdentifier = episode.Substring(1).ToLower().Split('e');
            var seasonNumber = Convert.ToInt32(epIdentifier[0]);
            var episodeNumber = Convert.ToInt32(epIdentifier[1]);
            var searchStringForEpisode = seasonNumber + "x" + episodeNumber;
            var searchResultUrl = (showName + " " + searchStringForEpisode).ToSearchUrl();
            var links = SearchForLinksForShow(searchResultUrl)
                .Where(l => l.SeasonEpisode.Equals(episode, StringComparison.InvariantCultureIgnoreCase));
            AddFoundLinksToStore(links);
        }
    }
}