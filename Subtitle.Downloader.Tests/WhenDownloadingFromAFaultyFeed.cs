using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Subtitle.Common;
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

        [Test]
        public void IfTheDownloadFailsTheExceptionShouldBeEaten()
        {
            var foundLinks = new List<FoundLink>();

            var feedLinks = new List<string>
            {
                "Exception.xml",
                "FaultyRss.xml",
            };

            var download = Substitute.For<IDownload>();

            download.From("FaultyRss.xml")
                .Returns(new ResourceDownload().From("FaultyRss.xml"));
            download.From("Exception.xml")
                .Returns(d => { throw new System.Net.WebException(); });
   
            var linkFinder = new LinkFinder(foundLinks, download);

            linkFinder.LookForLinksFromFeeds(feedLinks);

            foundLinks.Count.Should().Be(6);
        }
    }
}