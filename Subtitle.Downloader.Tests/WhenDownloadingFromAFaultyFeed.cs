using System;
using NUnit.Framework;

namespace Subtitle.Downloader.Tests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Provider.Addic7ed;

    [TestFixture]
    public class WhenDownloadingFromAFaultyFeed
    {
        [Test]
        public void TheFaultyItemShouldBeIgnored()
        {
            var foundLinks = new List<FoundLink>();

            var feedLinks = new List<string>
            {
                "FaultyRss.xml",
            };

            var download = new ResourceDownload();

            var linkFinder = new LinkFinder(foundLinks, download);

            linkFinder.LookForLinksFromFeeds(feedLinks);

            foundLinks.Count.Should().Be(6);
        }
    }
}
