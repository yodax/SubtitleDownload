using System.Collections.Generic;
using System.IO.Abstractions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Subtitle.Common;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader.Tests
{
    [TestFixture]
    public class WhenStartingASession
    {
        [Test]
        public void IfCheckCommandIsIssuedLookForLinksShouldBeCalled()
        {
            var download = Substitute.For<IDownload>();
            var fileSystem = Substitute.For<IFileSystem>();

            var notify = Substitute.For<INotifier>();

            var mediaFinder = Substitute.For<IMediaFinder>();

            var foundLinks = new List<FoundLink>();
            var feedLinks = new List<string>
            {
                "Link a"
            };
            var downloadedSubs = new List<DownloadedSub>();

            var subtitleSession = new SubtitleSession(
                download,
                fileSystem,
                notify,
                mediaFinder,
                foundLinks,
                feedLinks,
                downloadedSubs, -1, new List<string> {"Dutch", "English"}, new ListPersist(fileSystem));

            subtitleSession.StartNewSession(SessionAction.Check);

            download.Received().From(Arg.Is("Link a"), Arg.Any<string>());
        }

        [Test]
        public void IfDownloadCommandIsIssuedCheckShouldAlsoBeDone()
        {
            var download = Substitute.For<IDownload>();
            var fileSystem = Substitute.For<IFileSystem>();

            var notify = Substitute.For<INotifier>();

            var mediaFinder = Substitute.For<IMediaFinder>();

            var foundLinks = new List<FoundLink>();
            var feedLinks = new List<string>
            {
                "Link a"
            };
            var downloadedSubs = new List<DownloadedSub>();

            var subtitleSession = new SubtitleSession(
                download,
                fileSystem,
                notify,
                mediaFinder,
                foundLinks,
                feedLinks,
                downloadedSubs, -1, new List<string> {"Dutch", "English"}, new ListPersist(fileSystem));

            subtitleSession.StartNewSession(SessionAction.Download);

            download.Received().From(Arg.Is("Link a"), Arg.Any<string>());
        }

        [Test]
        public void IfDownloadCommandIsIssuedFeedLinkShouldBeRetrieved()
        {
            var download = Substitute.For<IDownload>();
            var fileSystem = Substitute.For<IFileSystem>();

            var notify = Substitute.For<INotifier>();

            var mediaFinder = Substitute.For<IMediaFinder>();

            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/The_Michael_J._Fox_Show/1/11/Christmas"
                }
            };

            var feedLinks = new List<string>
            {
                "Link a"
            };
            var downloadedSubs = new List<DownloadedSub>();

            var subtitleSession = new SubtitleSession(
                download,
                fileSystem,
                notify,
                mediaFinder,
                foundLinks,
                feedLinks,
                downloadedSubs, -1, new List<string> {"Dutch", "English"}, new ListPersist(fileSystem));

            subtitleSession.StartNewSession(SessionAction.Download);

            mediaFinder.Received().LookFor(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void IfSearchCommandIsIssuedForASpecificEpisodeLookForLinksFromEpisodeShouldBeCalled()
        {
            var resourceDownloader = new ResourceDownload();
            var download = Substitute.For<IDownload>();

            download.From(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => resourceDownloader.From(
                    "Search_Brickleberry.html"));

            var fileSystem = Substitute.For<IFileSystem>();
            var notify = Substitute.For<INotifier>();
            var mediaFinder = Substitute.For<IMediaFinder>();
            var foundLinks = new List<FoundLink>();
            var feedLinks = new List<string>();
            var downloadedSubs = new List<DownloadedSub>();

            var subtitleSession = new SubtitleSession(
                download,
                fileSystem,
                notify,
                mediaFinder,
                foundLinks,
                feedLinks,
                downloadedSubs, -1, new List<string> {"Dutch", "English"}, new ListPersist(fileSystem));

            subtitleSession.StartNewSession(SessionAction.Search, "Brickleberry", "S02E01");

            download.Received()
                .From(Arg.Is(@"http://www.addic7ed.com/search.php?search=Brickleberry+2x1&Submit=Search"),
                    Arg.Any<string>());
            foundLinks.Count.Should().Be(1);
        }

        [Test]
        public void IfSearchCommandIsIssuedLookForLinksFromShowShouldBeCalled()
        {
            var resourceDownloader = new ResourceDownload();
            var download = Substitute.For<IDownload>();

            download.From(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => resourceDownloader.From(
                    "Search_Brickleberry.html"));

            var fileSystem = Substitute.For<IFileSystem>();
            var notify = Substitute.For<INotifier>();
            var mediaFinder = Substitute.For<IMediaFinder>();
            var foundLinks = new List<FoundLink>();
            var feedLinks = new List<string>();
            var downloadedSubs = new List<DownloadedSub>();

            var subtitleSession = new SubtitleSession(
                download,
                fileSystem,
                notify,
                mediaFinder,
                foundLinks,
                feedLinks,
                downloadedSubs, -1, new List<string> {"Dutch", "English"}, new ListPersist(fileSystem));

            subtitleSession.StartNewSession(SessionAction.Search, "Brickleberry");

            download.Received()
                .From(Arg.Is(@"http://www.addic7ed.com/search.php?search=Brickleberry&Submit=Search"), Arg.Any<string>());
            foundLinks.Count.Should().Be(23);
        }
    }
}