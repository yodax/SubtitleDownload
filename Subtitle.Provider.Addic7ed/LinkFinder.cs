using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Subtitle.Common;

namespace Subtitle.Provider.Addic7ed
{
    public class LinkFinder
    {
        private readonly IDownload _download;
        private readonly List<FoundLink> _foundLinks;

        public LinkFinder(List<FoundLink> foundLinks, IDownload download)
        {
            _download = download;
            _foundLinks = foundLinks;
        }

        public void LookForLinksFromFeeds(IEnumerable<string> feedLinks)
        {
            var linksFoundFromFeeds = new List<FoundLink>();
            foreach (var feedLink in feedLinks)
            {
                try
                {
                    var feedContent = _download.From(feedLink);
                    linksFoundFromFeeds.AddRange(AddictedFeedReader
                        .GetAllLinksFrom(feedContent)
                        .Where(l => l.Contains("/serie/"))
                        .Select(s => new FoundLink
                        {
                            Link = s,
                            FoundOn = DateTime.Now
                        }));
                }
                catch (WebException){}
                
            }

            AddFoundLinksToStore(linksFoundFromFeeds);
        }

        private void AddFoundLinksToStore(IEnumerable<FoundLink> newlyFoundLinks)
        {
            var allLinks = _foundLinks.Union(newlyFoundLinks).ToList();
            _foundLinks.Clear();
            _foundLinks.AddRange(allLinks
                .GroupBy(l => l.Link.StripAddictedEpisode())
                .Select(grp => grp.OrderByDescending(x => x.IgnoreAge).ThenBy(x => x.FoundOn)
                    .First()));

            var badLinks = new List<FoundLink>();

            _foundLinks.ForEach(l =>
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

            badLinks.ForEach(badLink => _foundLinks.Remove(badLink));
        }

        public void LookForLinksFromShow(string showName)
        {
            var links = SearchForLinksForShow(showName.ToSearchUrl());
            AddFoundLinksToStore(links);
        }

        private IEnumerable<FoundLink> SearchForLinksForShow(string searchResultUrl)
        {
            var searchUrl = searchResultUrl;
            var pageContent = _download.From(searchUrl);

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