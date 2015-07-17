using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader.Tests
{
    [TestFixture]
    public class WhenPersistingLists
    {
        private static MockFileSystem CreateFileSystem()
        {
            return new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {MockUnixSupport.Path(@"c:\downloadTool\tool.txt"), new MockFileData("")}
            }, MockUnixSupport.Path(@"c:\downloadTool\"));
        }

        [Test]
        public void IfFoundLinksContainsNoItemsNoItemsShouldBeStored()
        {
            var foundLinks = new List<FoundLink>();

            var fileSystem = CreateFileSystem();

            foundLinks.Store("FoundLinks", fileSystem);
            var newFoundLinks = new List<FoundLink>();
            newFoundLinks.Load("FoundLinks", fileSystem);

            fileSystem.FileExists(MockUnixSupport.Path(@"c:\downloadTool\FoundLinks.xml")).Should().BeTrue();
            newFoundLinks.Count().Should().Be(0);
        }

        [Test]
        public void IfFoundLinksContainsOneItemOneItemShouldBeStored()
        {
            var dateTime = DateTime.Now;
            var foundLinks = new List<FoundLink>
            {
                new FoundLink {FoundOn = dateTime, Link = "Stored link"}
            };

            var fileSystem = CreateFileSystem();

            foundLinks.Store("FoundLinks", fileSystem);
            var newFoundLinks = new List<FoundLink>().Load("FoundLinks", fileSystem);

            fileSystem.FileExists(MockUnixSupport.Path(@"c:\downloadTool\FoundLinks.xml")).Should().BeTrue();
            newFoundLinks.Count().Should().Be(1);
            newFoundLinks.ElementAt(0).FoundOn.Should().Be(dateTime);
            newFoundLinks.ElementAt(0).Link.Should().Be("Stored link");
        }

        [Test]
        public void IfStoreIsNotThereYetAnEmptyListShouldBeReturned()
        {
            var downloadedSubs = new List<DownloadedSub>();
            var fileSystem = CreateFileSystem();

            downloadedSubs.Load("DownloadedSubs", fileSystem);

            downloadedSubs.Count().Should().Be(0);
        }

        [Test]
        public void IfTheStoreWasAlreadyPersistedItShouldPersistAgain()
        {
            var dateTime = DateTime.Now;
            var foundLinks = new List<FoundLink>
            {
                new FoundLink {FoundOn = dateTime, Link = "Stored link"}
            };

            var fileSystem = CreateFileSystem();

            foundLinks.Store("FoundLinks", fileSystem);
            var newFoundLinks = new List<FoundLink>().Load("FoundLinks", fileSystem);

            newFoundLinks.Add(new FoundLink {Link = "Added link"});
            newFoundLinks.Store("FoundLinks", fileSystem);
            newFoundLinks.Load("FoundLinks", fileSystem);

            newFoundLinks.Count.Should().Be(2);
        }

        [Test]
        public void ShouldWorkForDownloadedSub()
        {
            var downloadedSubs = new List<DownloadedSub>
            {
                new DownloadedSub {Link = "Stored link", On = DateTime.Now}
            };

            var fileSystem = CreateFileSystem();

            downloadedSubs.Store("DownloadedSubs", fileSystem);

            downloadedSubs.Load("DownloadedSubs", fileSystem);

            downloadedSubs.Count.Should().Be(1);
        }
    }
}