﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Subtitle.Common;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenLookingForLinksForAnEpisode
    {
        [Test]
        public void IfADirectLinkIsFoundItIsAddedToFoundLinks()
        {
            var foundLinks = new List<FoundLink>();

            var resourceDownloader = new ResourceDownload();
            var mockDownloader = Substitute.For<IDownload>();

            mockDownloader.From(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => resourceDownloader.From(
                    "Anger Management - 02x43 - Charlie Loses His Virginity Again.html"));

            var linkFinder = new LinkFinder(foundLinks, mockDownloader);

            linkFinder.LookForLinksForEpisode("Anger Management", "s02e43");

            foundLinks.Count.Should().Be(1);
        }

        [Test]
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

            linkFinder.LookForLinksForEpisode("Brickleberry", "s02e01");

            foundLinks.Count.Should().Be(1 + 1);

            foundLinks.ElementAt(1).ShowName.Should().Be("Brickleberry");
        }
    }
}