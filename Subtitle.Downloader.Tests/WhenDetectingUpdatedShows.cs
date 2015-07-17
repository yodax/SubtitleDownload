using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader.Tests
{
    [TestFixture]
    public class WhenDetectingUpdatedShows
    {
        [SetUp]
        public void Initialize()
        {
            GivenNoFoundLinks();
            GivenThreeSampleFeedLinks();
            GivenALinkFinder();
        }

        private List<string> _feedLinks;
        private List<FoundLink> _foundLinks;
        private LinkFinder _linkFinder;

        private void GivenTheFoundLinkWithAFoundOnTime(string link, DateTime foundOn)
        {
            var firstDateTime = foundOn;
            _foundLinks.Add(new FoundLink
            {
                Link = link,
                FoundOn = firstDateTime
            });
        }

        private void WhenILookForLinks()
        {
            _linkFinder.LookForLinksFromFeeds(_feedLinks);
        }

        private void GivenTheFoundLink(string link)
        {
            _foundLinks.Add(new FoundLink
            {
                Link = link
            });
        }

        private void GivenALinkFinder()
        {
            var download = new ResourceDownload();

            _linkFinder = new LinkFinder(_foundLinks, download);
        }

        private void GivenNoFoundLinks()
        {
            _foundLinks = new List<FoundLink>();
        }

        private void GivenThreeSampleFeedLinks()
        {
            _feedLinks = new List<string>
            {
                "Addic7edLastNewVersions.xml",
                "Addic7edLastUploaded.xml",
                "AddictedHotspot.xml"
            };
        }

        [Test]
        public void IfALinkWasAlreadyPresentAndItWasFoundAgainItShouldNotBeAdded()
        {
            GivenTheFoundLink("http://www.addic7ed.com/serie/South_Park/17/8/A_Song_of_Ass_and_Fire7");

            WhenILookForLinks();

            _foundLinks.Count.Should().Be(10);
        }

        [Test]
        public void IfALinkWasAlreadyPresentAndItWasFoundAgainItShouldNotBeAddedButTheMostRecentTimeShouldBeUsed()
        {
            const string link = "http://www.addic7ed.com/serie/South_Park/17/8/A_Song_of_Ass_and_Fire";
            var foundOn = new DateTime(2000, 1, 1);
            GivenTheFoundLinkWithAFoundOnTime(link, foundOn);

            WhenILookForLinks();

            _foundLinks.Count.Should().Be(10);

            _foundLinks.First(l => l.Link.StartsWith(link)).FoundOn.Should().Be(foundOn);
        }

        [Test]
        public void IfALinkWasAlreadyPresentItShouldStillBeAvailable()
        {
            GivenTheFoundLink("http://www.addic7ed.com/serie/Non_existing_show/17/8/A");

            WhenILookForLinks();

            _foundLinks.Count.Should().Be(11);
        }

        [Test]
        public void IfALinkWasAlreadyPresentWithADifferentEpisodeNameAndItWasFoundAgainItShouldNotBeAdded()
        {
            GivenTheFoundLink("http://www.addic7ed.com/serie/South_Park/17/8/A_Song_of_Ass_and_Fire117");

            WhenILookForLinks();

            _foundLinks.Count.Should().Be(10);
        }

        [Test]
        public void IfAnEpisodeLinkIsFoundItShouldBeStored()
        {
            _feedLinks.RemoveRange(1, 2);

            WhenILookForLinks();

            _foundLinks.Count.Should().Be(7);
        }

        [Test]
        public void IfAnEpisodeLinkIsFoundMutipleTimesItShouldOnlyBeStoredOnce()
        {
            WhenILookForLinks();

            _foundLinks.Count.Should().Be(10);
        }

        [Test]
        public void TheShowNameAndTheEpisodeShouldBeStored()
        {
            WhenILookForLinks();

            var southPark = _foundLinks.First(l => l.Link.Contains("South"));

            southPark.ShowName.Should().Be("South Park");
            southPark.SeasonEpisode.Should().Be("S17E08");
        }
    }
}