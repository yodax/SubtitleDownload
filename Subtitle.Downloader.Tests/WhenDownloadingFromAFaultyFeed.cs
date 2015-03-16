using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Subtitle.Downloader.Tests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Provider.Addic7ed;

    [TestClass]
    public class WhenDownloadingFromAFaultyFeed
    {
        [TestMethod]
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
