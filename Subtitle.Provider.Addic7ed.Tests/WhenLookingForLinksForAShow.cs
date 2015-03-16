namespace Subtitle.Provider.Addic7ed.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class WhenLookingForLinksForAShow
    {
        [TestMethod]
        public void IfLinksAreFoundTheyShouldBeAddedToFoundLinks()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    ShowName = "Old Show",
                    Link = "http://www.addic7ed.com/serie/OldShow/2/8/Little_Boy_Malloy"
                }
            };

            var resourceDownloader = new ResourceDownload();
            var mockDownloader = Substitute.For<IDownload>();

            mockDownloader.From(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => resourceDownloader.From(
                    "Search_Brickleberry.html"));

            var linkFinder = new LinkFinder(foundLinks, mockDownloader);

            linkFinder.LookForLinksFromShow("Brickleberry");

            foundLinks.Count.Should().Be(23 + 1);

            foundLinks.ElementAt(23).ShowName.Should().Be("Brickleberry");
        }

        [TestMethod]
        public void IfLinksAreAlreadyPresentIgnoreAgeShouldStillBeTrue()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    FoundOn = DateTime.MinValue,
                    ShowName = "Brickleberry",
                    Link = "http://www.addic7ed.com/serie/Brickleberry/1/2/2_Weeks_Notice",
                    IgnoreAge = false
                }
            };

            var resourceDownloader = new ResourceDownload();
            var mockDownloader = Substitute.For<IDownload>();

            mockDownloader.From(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => resourceDownloader.From(
                    "Search_Brickleberry.html"));

            var linkFinder = new LinkFinder(foundLinks, mockDownloader);

            linkFinder.LookForLinksFromShow("Brickleberry");

            foundLinks.Count.Should().Be(23);

            foundLinks.Count(link => link.IgnoreAge == false).Should().Be(0);
        }
    }
}