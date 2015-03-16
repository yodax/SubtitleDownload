using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Subtitle.Downloader.Tests
{
    [TestClass]
    public class WhenRemovingOlderDownloadedSubsFromCollection
    {
        private readonly DateTime twentyNineDaysAgo = DateTime.Now.AddDays(-29);
        private readonly DateTime thirtyOneDaysAgo = DateTime.Now.AddDays(-31);

        [TestMethod]
        public void IfTheDownloadedSubsContainNoOlderDownloadsNothingShouldBePurged()
        {
            var downloadedSubs = new List<DownloadedSub>
                        {
                            new DownloadedSub
                            {
                                On = DateTime.Now,
                            }
                        };

            downloadedSubs.RemoveOlderThan(30).Should().HaveCount(1);
        }

        [TestMethod]
        public void IfTheDownloadedSubsContainsOlderDownloadsThanTheyShouldBePurged()
        {
            var downloadedSubs = new List<DownloadedSub>
                        {
                            new DownloadedSub
                            {
                                On = thirtyOneDaysAgo,
                            }
                        };

            downloadedSubs.RemoveOlderThan(30).Should().BeEmpty();
        }

        [TestMethod]
        public void IfTheDownloadedSubsContainBothOlderAndNewerDownloadsItShouldContainOnlyNewerDownloads()
        {
            var downloadedSubs = new List<DownloadedSub>
                        {
                            new DownloadedSub
                            {
                                On = thirtyOneDaysAgo,
                            },
                            new DownloadedSub
                            {
                                Link = "Sub to keep",
                                On = DateTime.Today,
                            },
                            new DownloadedSub
                            {
                                On = thirtyOneDaysAgo,
                            },
                            new DownloadedSub
                            {
                                Link = "Sub to keep 2",
                                On = twentyNineDaysAgo,
                            },
                            new DownloadedSub
                            {
                                On = thirtyOneDaysAgo,
                            },
                        };

            downloadedSubs.RemoveOlderThan(30).Should().HaveCount(2);
            downloadedSubs.ElementAt(0).Link.Should().Be("Sub to keep");
            downloadedSubs.ElementAt(1).Link.Should().Be("Sub to keep 2");
        }
    }
}