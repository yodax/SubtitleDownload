using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Subtitle.Common;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader.Tests
{
    [TestFixture]
    public class WhenDownloadingSubtitles
    {
        private static MockFileSystem CheckAndDownloadASubtitle(List<string> orderedAllowedLanguages)
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = Substitute.For<IMediaFinder>();
            mediaFinder.LookFor(Arg.Any<string>(), Arg.Any<string>())
                .Returns(new List<Media>
                {
                    new Media
                    {
                        Name = @"Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                        Path = @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv"
                    }
                });

            var mockDownloader = CreateMockDownloader();

            var subtitleDownloader = new SubtitleDownloader(
                mediaFinder,
                mockDownloader,
                fileSystem,
                new List<DownloadedSub>(),
                Substitute.For<INotifier>(), -1, orderedAllowedLanguages);

            subtitleDownloader.For(foundLinks);
            return fileSystem;
        }

        private static IDownload CreateMockDownloader()
        {
            var resourceDownloader = new ResourceDownload();
            var mockDownloader = Substitute.For<IDownload>();

            mockDownloader.From(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => resourceDownloader.From(
                    "Anger Management - 02x43 - Charlie Loses His Virginity Again.html"),
                    x => resourceDownloader.From("Subtitle Killers.srt"));
            return mockDownloader;
        }

        private static MockFileSystem CheckForDownloadWithSubAlreadyDownloaded(string downloadedFor)
        {
            var mediaLocation = downloadedFor;

            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var previouslyDownloadedSubs = new List<DownloadedSub>
            {
                new DownloadedSub
                {
                    On = new DateTime(2013, 1, 1),
                    Link = "http://www.addic7ed.com/Subtitle Killers.srt",
                    For = mediaLocation
                }
            };

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var resourceDownloader = CreateMockDownloader();

            var subtitleDownloader = new SubtitleDownloader(mediaFinder, resourceDownloader, fileSystem,
                previouslyDownloadedSubs, Substitute.For<INotifier>(), -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);
            return fileSystem;
        }

        [Test]
        public void AfterDownloadingCompletesFoundLinksShouldBeCleared()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var download = CreateMockDownloader();
            var subtitleDownloader = new SubtitleDownloader(mediaFinder, download, fileSystem,
                new List<DownloadedSub>(), Substitute.For<INotifier>(), -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            foundLinks.Should().BeEmpty();
        }

        [Test]
        public void IfADifferentLanguageSubtitleExistsLeaveIt()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            //var oldSubLocation = @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.en.srt";
            var oldSubLocation =
                MockUnixSupport.Path(@"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.en.srt");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    //@"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    MockUnixSupport.Path(@"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv"),
                    new MockFileData("")
                },
                {
                    oldSubLocation,
                    new MockFileData("Old sub!")
                }
            });

            var mediaFinder = new MediaFinder(MockUnixSupport.Path(@"c:\video"), fileSystem);

            var resourceDownloader = CreateMockDownloader();

            var subtitleDownloader = new SubtitleDownloader(mediaFinder, resourceDownloader, fileSystem,
                new List<DownloadedSub>(), Substitute.For<INotifier>(), -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            var srtLocation =
                MockUnixSupport.Path(@"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt");

            foreach (var directory in fileSystem.AllDirectories)
            {
                    Debug.WriteLine(directory);
            }

            foreach (var file in fileSystem.AllFiles)
            {
                Debug.WriteLine(file);
            }

            fileSystem.FileExists(srtLocation)
                .Should().BeTrue("Path should exist");

            fileSystem.File.ReadAllText(srtLocation)
                .TrimEnd()
                .Equals("Sub text", StringComparison.InvariantCulture)
                .Should()
                .BeTrue("Contents of file should match");

            fileSystem.FileExists(oldSubLocation)
                .Should().BeTrue();

            fileSystem.File.ReadAllText(oldSubLocation)
                .TrimEnd()
                .Equals("Old sub!", StringComparison.InvariantCulture)
                .Should()
                .BeTrue("Old file should contain correct content");
        }

        [Test]
        public void IfAFoundLinkHasMatchingMediaDownloadTheSubtitle()
        {
            var fileSystem = CheckAndDownloadASubtitle(new List<string> {"Dutch", "English"});

            const string srtLocation =
                @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt";
            fileSystem.FileExists(srtLocation)
                .Should().BeTrue();

            fileSystem.File.ReadAllText(srtLocation)
                .TrimEnd()
                .Equals("Sub text", StringComparison.InvariantCulture)
                .Should()
                .BeTrue();
        }

        [Test]
        public void IfAFoundLinkWasFromASearchUrlDownloadTheSubtitle()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/search.php?search=Anger+Management+2x43&Submit=Search"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = Substitute.For<IMediaFinder>();
            mediaFinder.LookFor(Arg.Any<string>(), Arg.Any<string>())
                .Returns(new List<Media>
                {
                    new Media
                    {
                        Name = @"Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                        Path = @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv"
                    }
                });

            var mockDownloader = CreateMockDownloader();

            var subtitleDownloader = new SubtitleDownloader(
                mediaFinder,
                mockDownloader,
                fileSystem,
                new List<DownloadedSub>(),
                Substitute.For<INotifier>(), -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            const string srtLocation =
                @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt";
            fileSystem.FileExists(srtLocation)
                .Should().BeTrue();

            fileSystem.File.ReadAllText(srtLocation)
                .TrimEnd()
                .Equals("Sub text", StringComparison.InvariantCulture)
                .Should()
                .BeTrue();
        }

        [Test]
        public void IfALinkIsDownloadedANotificationShouldBeSent()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var resourceDownloader = CreateMockDownloader();

            var notifier = Substitute.For<INotifier>();

            var subtitleDownloader = new SubtitleDownloader(
                mediaFinder,
                resourceDownloader,
                fileSystem,
                new List<DownloadedSub>(),
                notifier, -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            notifier.Received()
                .ForDownloadedSubtitle(Arg.Any<EpisodePage>(), Arg.Any<SubtitleVersion>(),
                    Arg.Any<Provider.Addic7ed.Subtitle>(),
                    Arg.Any<SubtitleLink>(),
                    Arg.Is(@"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt"),
                    Arg.Any<string>());
        }

        [Test]
        public void IfALinksShowNameDoesNotHaveMatchingMediaDoNotDownloadPage()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Atlantis_%282013%29/1/10/The_Price_of_Hope"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var download = Substitute.For<IDownload>();

            var subtitleDownloader = new SubtitleDownloader(
                mediaFinder,
                download,
                fileSystem,
                new List<DownloadedSub>(),
                Substitute.For<INotifier>(), -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            download.DidNotReceive().From(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void IfASubtitleExistsOverwriteIt()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                },
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt",
                    new MockFileData("Old sub!")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var resourceDownloader = CreateMockDownloader();

            var subtitleDownloader = new SubtitleDownloader(mediaFinder, resourceDownloader, fileSystem,
                new List<DownloadedSub>(), Substitute.For<INotifier>(), -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            const string srtLocation =
                @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt";
            fileSystem.FileExists(srtLocation)
                .Should().BeTrue();

            fileSystem.File.ReadAllText(srtLocation)
                .TrimEnd()
                .Equals("Sub text", StringComparison.InvariantCulture)
                .Should()
                .BeTrue();
        }

        [Test]
        public void IfASubtitleHasAlreadyBeenDownloadedButItIsForDifferentMediaDownloadAgain()
        {
            var fileSystem =
                CheckForDownloadWithSubAlreadyDownloaded(
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-BOINK.mkv");

            const string srtLocation =
                @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt";
            fileSystem.FileExists(srtLocation)
                .Should().BeTrue();
        }

        [Test]
        public void IfASubtitleHasAlreadyBeenDownloadedButItIsUnknownForWhichMediaDontDownloadAgain()
        {
            var fileSystem = CheckForDownloadWithSubAlreadyDownloaded(string.Empty);

            const string srtLocation =
                @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt";
            fileSystem.FileExists(srtLocation)
                .Should().BeFalse();
        }

        [Test]
        public void IfASubtitleHasAlreadyBeenDownloadedDontDownloadAgain()
        {
            var fileSystem =
                CheckForDownloadWithSubAlreadyDownloaded(
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv");

            const string srtLocation =
                @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt";
            fileSystem.FileExists(srtLocation)
                .Should().BeFalse();
        }

        [Test]
        public void IfASubtitleIsDownloadedALanguageSuffixShouldBeAdded()
        {
            var fileSystem = CheckAndDownloadASubtitle(new List<string> {"Dutch", "English"});

            const string srtLocation =
                @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.nl.srt";
            fileSystem.FileExists(srtLocation)
                .Should().BeTrue();

            fileSystem.File.ReadAllText(srtLocation)
                .TrimEnd()
                .Equals("Sub text", StringComparison.InvariantCulture)
                .Should()
                .BeTrue();
        }

        [Test]
        public void IfASubtitleIsDownloadedAnEnglishLanguageSuffixShouldBeAdded()
        {
            var fileSystem = CheckAndDownloadASubtitle(new List<string> {"English"});

            const string srtLocation =
                @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.en.srt";
            fileSystem.FileExists(srtLocation)
                .Should().BeTrue();

            fileSystem.File.ReadAllText(srtLocation)
                .TrimEnd()
                .Equals("Sub text", StringComparison.InvariantCulture)
                .Should()
                .BeTrue();
        }

        [Test]
        public void IfASubtitleIsDownloadedItShouldBeAddedToTheFoundLinks()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var mediaLocation = @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    mediaLocation,
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);
            var resourceDownloader = CreateMockDownloader();
            var previouslyDownloadedSubs = new List<DownloadedSub>();
            var subtitleDownloader = new SubtitleDownloader(mediaFinder, resourceDownloader, fileSystem,
                previouslyDownloadedSubs, Substitute.For<INotifier>(), -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            previouslyDownloadedSubs.Should().NotBeEmpty();
            previouslyDownloadedSubs.Count.Should().Be(1);
            previouslyDownloadedSubs.ElementAt(0).On.Should().BeAfter(DateTime.MinValue);
            previouslyDownloadedSubs.ElementAt(0).Link.Should().Be("http://www.addic7ed.com/Subtitle Killers.srt");
            previouslyDownloadedSubs.ElementAt(0).For.Should().Be(mediaLocation);
        }

        [Test]
        public void IfDownloadCountExceededAfterDownloadingCompletesFoundLinksShouldContainMissedDownloads()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                },
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                },
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var resourceDownloader = new ResourceDownload();
            var mockDownloader = Substitute.For<IDownload>();

            mockDownloader.From(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => resourceDownloader.From(
                    "Anger Management - 02x43 - Charlie Loses His Virginity Again.html"),
                    x => resourceDownloader.From("Subtitle Killers.srt"),
                    x => resourceDownloader.From(
                        "Anger Management - 02x43 - Charlie Loses His Virginity Again Download Exceeded.html"),
                    x => resourceDownloader.From("ExceededDownload.srt"));

            var notifier = Substitute.For<INotifier>();
            var subtitleDownloader = new SubtitleDownloader(mediaFinder, mockDownloader, fileSystem,
                new List<DownloadedSub>(), notifier, -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            // 3 links to start, one success on exceeds, other ignored
            foundLinks.Count.Should().Be(2);
            notifier.Received().ForDownloadCountExceeded();
        }

        [Test]
        public void IfNoMatchingSubtitleWasFoundNothingShouldBeDownloaded()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-NOGROUP.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var resourceDownloader = CreateMockDownloader();

            var notifier = Substitute.For<INotifier>();

            var previouslyDownloadedSubs = new List<DownloadedSub>();
            var subtitleDownloader = new SubtitleDownloader(
                mediaFinder,
                resourceDownloader,
                fileSystem,
                previouslyDownloadedSubs,
                notifier, -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            previouslyDownloadedSubs.Should().BeEmpty();
            notifier.DidNotReceive()
                .ForException(Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<EpisodePage>(),
                    Arg.Any<Provider.Addic7ed.Subtitle>(), Arg.Any<SubtitleLink>());
        }

        [Test]
        public void IfTheOldestAgeIsLargerThanSpecifiedButAgeShouldBeIgnoredDownloadSub()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again",
                    IgnoreAge = true
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var download = CreateMockDownloader();

            var previouslyDownloadedSubs = new List<DownloadedSub>();

            var subtitleDownloader = new SubtitleDownloader(mediaFinder, download, fileSystem,
                previouslyDownloadedSubs, Substitute.For<INotifier>(), 6, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            previouslyDownloadedSubs.Count.Should().Be(1);

            foundLinks.Should().BeEmpty();
        }

        [Test]
        public void IfTheOldestAgeIsLargerThanSpecifiedDontDownloadEvenWhenNewerVersionsArePresent()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var download = CreateMockDownloader();

            var previouslyDownloadedSubs = new List<DownloadedSub>();

            var subtitleDownloader = new SubtitleDownloader(mediaFinder, download, fileSystem,
                previouslyDownloadedSubs, Substitute.For<INotifier>(), 6, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            previouslyDownloadedSubs.Count.Should().Be(0);

            foundLinks.Should().BeEmpty();
        }

        [Test]
        public void IfTheOldestIsSpecifiedAtMinusOneIgnoreTheParameter()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var download = CreateMockDownloader();

            var previouslyDownloadedSubs = new List<DownloadedSub>();

            var subtitleDownloader = new SubtitleDownloader(mediaFinder, download, fileSystem,
                previouslyDownloadedSubs, Substitute.For<INotifier>(), -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            previouslyDownloadedSubs.Count.Should().Be(1);
        }

        [Test]
        public void WhenAnEpisodePageNoLongerExistsANotificationShouldBeSent()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var resourceDownloader = Substitute.For<IDownload>();
            var exception = new EpisodePageNoLongerExists("z");
            resourceDownloader.From(Arg.Any<string>(), Arg.Any<string>()).Returns(x => { throw exception; });

            var notifier = Substitute.For<INotifier>();

            var subtitleDownloader = new SubtitleDownloader(
                mediaFinder,
                resourceDownloader,
                fileSystem,
                new List<DownloadedSub>(),
                notifier, -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            notifier.DidNotReceive()
                .ForException(Arg.Any<Exception>(), Arg.Any<string>(), Arg.Is<EpisodePage>(p => p == null),
                    Arg.Is<Provider.Addic7ed.Subtitle>(s => s == null), Arg.Is<SubtitleLink>(l => l == null));
            foundLinks.Count.Should().Be(0);
        }

        [Test]
        public void WhenAnExceptionIsThrowDuringDownloadANotificationShouldBeSent()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var resourceDownloader = new ResourceDownload();
            var mockDownloader = Substitute.For<IDownload>();
            var exception = new Exception();
            mockDownloader.From(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => resourceDownloader.From(
                    "Anger Management - 02x43 - Charlie Loses His Virginity Again.html"),
                    x => { throw exception; });

            var notifier = Substitute.For<INotifier>();

            var subtitleDownloader = new SubtitleDownloader(
                mediaFinder,
                mockDownloader,
                fileSystem,
                new List<DownloadedSub>(),
                notifier, -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            notifier.Received()
                .ForException(Arg.Is(exception), Arg.Any<string>(),
                    Arg.Is<EpisodePage>(p => p.ShowName == "Anger Management"),
                    Arg.Any<Provider.Addic7ed.Subtitle>(), Arg.Any<SubtitleLink>());
        }

        [Test]
        public void WhenAnExceptionIsThrowDuringPageExtractionANotificationShouldBeSent()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var resourceDownloader = Substitute.For<IDownload>();
            var exception = new Exception();
            resourceDownloader.From(Arg.Any<string>(), Arg.Any<string>()).Returns(x => { throw exception; });

            var notifier = Substitute.For<INotifier>();

            var subtitleDownloader = new SubtitleDownloader(
                mediaFinder,
                resourceDownloader,
                fileSystem,
                new List<DownloadedSub>(),
                notifier, -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            notifier.Received()
                .ForException(Arg.Any<Exception>(), Arg.Any<string>(), Arg.Is<EpisodePage>(p => p == null),
                    Arg.Is<Provider.Addic7ed.Subtitle>(s => s == null), Arg.Is<SubtitleLink>(l => l == null));
            foundLinks.Count.Should().Be(0);
        }

        [Test]
        public void WhenDownloadingPassPageAsReferer()
        {
            var foundLinks = new List<FoundLink>
            {
                new FoundLink
                {
                    Link = "http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                }
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    @"c:\video\Anger Management\S02E43\Anger.Management.S02E43.720p.HDTV-KILLERS.mkv",
                    new MockFileData("")
                }
            });

            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);

            var mockDownloader = CreateMockDownloader();

            var notifier = Substitute.For<INotifier>();

            var subtitleDownloader = new SubtitleDownloader(
                mediaFinder,
                mockDownloader,
                fileSystem,
                new List<DownloadedSub>(),
                notifier, -1, new List<string> {"Dutch", "English"});

            subtitleDownloader.For(foundLinks);

            mockDownloader.Received()
                .From(Arg.Any<string>(),
                    Arg.Is("http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"));
        }
    }
}