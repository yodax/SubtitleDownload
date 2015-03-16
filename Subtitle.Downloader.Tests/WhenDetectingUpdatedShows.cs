namespace Subtitle.Downloader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Provider.Addic7ed;

    [TestClass]
    public class WhenDetectingUpdatedShows
    {
        private List<string> feedLinks;
        private List<FoundLink> foundLinks;
        private LinkFinder linkFinder;

        [TestInitialize]
        public void Initialize()
        {
            GivenNoFoundLinks();
            GivenThreeSampleFeedLinks();
            GivenALinkFinder();
        }

        [TestMethod]
        public void IfALinkWasAlreadyPresentAndItWasFoundAgainItShouldNotBeAdded()
        {
            GivenTheFoundLink("http://www.addic7ed.com/serie/South_Park/17/8/A_Song_of_Ass_and_Fire7");

            WhenILookForLinks();

            foundLinks.Count.Should().Be(10);
        }

        [TestMethod]
        public void IfALinkWasAlreadyPresentAndItWasFoundAgainItShouldNotBeAddedButTheMostRecentTimeShouldBeUsed()
        {
            const string link = "http://www.addic7ed.com/serie/South_Park/17/8/A_Song_of_Ass_and_Fire";
            var foundOn = new DateTime(2000, 1, 1);
            GivenTheFoundLinkWithAFoundOnTime(link, foundOn);

            WhenILookForLinks();

            foundLinks.Count.Should().Be(10);

            foundLinks.First(l => l.Link.StartsWith(link)).FoundOn.Should().Be(foundOn);
        }

        [TestMethod]
        public void IfALinkWasAlreadyPresentItShouldStillBeAvailable()
        {
            GivenTheFoundLink("http://www.addic7ed.com/serie/Non_existing_show/17/8/A");

            WhenILookForLinks();

            foundLinks.Count.Should().Be(11);
        }

        [TestMethod]
        public void IfALinkWasAlreadyPresentWithADifferentEpisodeNameAndItWasFoundAgainItShouldNotBeAdded()
        {
            GivenTheFoundLink("http://www.addic7ed.com/serie/South_Park/17/8/A_Song_of_Ass_and_Fire117");

            WhenILookForLinks();

            foundLinks.Count.Should().Be(10);
        }

        [TestMethod]
        public void IfAnEpisodeLinkIsFoundItShouldBeStored()
        {
            feedLinks.RemoveRange(1, 2);

            WhenILookForLinks();

            foundLinks.Count.Should().Be(7);
        }

        [TestMethod]
        public void IfAnEpisodeLinkIsFoundMutipleTimesItShouldOnlyBeStoredOnce()
        {
            WhenILookForLinks();

            foundLinks.Count.Should().Be(10);
        }

        [TestMethod]
        public void TheShowNameAndTheEpisodeShouldBeStored()
        {
            WhenILookForLinks();

            var southPark = foundLinks.First(l => l.Link.Contains("South"));

            southPark.ShowName.Should().Be("South Park");
            southPark.SeasonEpisode.Should().Be("S17E08");
        }

        private void GivenTheFoundLinkWithAFoundOnTime(string link, DateTime foundOn)
        {
            var firstDateTime = foundOn;
            foundLinks.Add(new FoundLink
            {
                Link = link,
                FoundOn = firstDateTime
            });
        }

        private void WhenILookForLinks()
        {
            linkFinder.LookForLinksFromFeeds(feedLinks);
        }

        private void GivenTheFoundLink(string link)
        {
            foundLinks.Add(new FoundLink
            {
                Link = link
            });
        }

        private void GivenALinkFinder()
        {
            var download = new ResourceDownload();

            linkFinder = new LinkFinder(foundLinks, download);
        }

        private void GivenNoFoundLinks()
        {
            foundLinks = new List<FoundLink>();
        }

        private void GivenThreeSampleFeedLinks()
        {
            feedLinks = new List<string>
            {
                "Addic7edLastNewVersions.xml",
                "Addic7edLastUploaded.xml",
                "AddictedHotspot.xml"
            };
        }
    }
}