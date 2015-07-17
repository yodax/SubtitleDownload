using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader.Tests
{
    [TestFixture]
    public class WhenDownloadingFromAFaultyFeed
    {
        [Test]
        public void TheFaultyItemShouldBeIgnored()
        {
            var foundLinks = new List<FoundLink>();

            var feedLinks = new List<string>
            {
                "FaultyRss.xml"
            };

            var download = new ResourceDownload();

            var linkFinder = new LinkFinder(foundLinks, download);

            linkFinder.LookForLinksFromFeeds(feedLinks);

            foundLinks.Count.Should().Be(6);
        }
    }
}