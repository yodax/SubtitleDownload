namespace Subtitle.Downloader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions.TestingHelpers;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenLookingForMediaForAShow
    {
        [Test]
        public void IfShowAndEpisodeExistReturnTheMedia()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")}
            });
            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E01").ToList();

            foundMedia.Count().Should().Be(1);
            foundMedia.First().Name.Should().Be("ShowName.S01E01.720p.HDTV-GROUP.mkv");
        }

        [Test]
        public void IfShowExistButAlsoAnNfoExistOnlyTheShowShouldBeReturned()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.nfo", new MockFileData("")}
            });
            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E01").ToList();

            foundMedia.Count().Should().Be(1);
            foundMedia.First().Name.Should().Be("ShowName.S01E01.720p.HDTV-GROUP.mkv");
        }

        [Test]
        public void IfShowExistsButEpisodeDoesNotNoMediaShouldBeFound()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.nfo", new MockFileData("")}
            });
            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E02").ToList();

            foundMedia.Should().BeEmpty();
        }

        [Test]
        public void OnlyMediaWithAProperMediaExtensionShouldBeFound()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.avi", new MockFileData("")},
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.nfo", new MockFileData("")},
            });
            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E01").ToList();

            foundMedia.Count().Should().Be(2);
            foundMedia.ElementAt(0).Name.Should().Be("ShowName.S01E01.720p.HDTV-GROUP.mkv");
            foundMedia.ElementAt(1).Name.Should().Be("ShowName.S01E01.720p.HDTV-GROUP.avi");
        }

        [Test]
        public void FullPathShouldBeAssignedCorrectly()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
                {@"c:\video\ShowName Us\S01E01\ShowName.Us.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
            });
            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E01").ToList();

            foundMedia.Count().Should().Be(2);
            foundMedia.ElementAt(0).Path.Should().Be(@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv");
            foundMedia.ElementAt(1)
                .Path.Should()
                .Be(@"c:\video\ShowName Us\S01E01\ShowName.Us.S01E01.720p.HDTV-GROUP.mkv");
        }

        [Test]
        public void OnlyMediaForTheCorrectShowShouldBeFound()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
                {@"c:\video\OtherShow\S01E01\OtherShow.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
            });
            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E01").ToList();

            foundMedia.Count().Should().Be(1);
            foundMedia.ElementAt(0).Name.Should().StartWith("ShowName");
        }

        [Test]
        public void ShowNameShouldUseAproximateComparison()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
                {@"c:\video\OtherShow\S01E01\OtherShow.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
                {@"c:\video\ShowName Us\S01E01\ShowName.Us.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
            });
            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E01").ToList();

            foundMedia.Count().Should().Be(2);
            foundMedia.ElementAt(0).Name.Should().Be("ShowName.S01E01.720p.HDTV-GROUP.mkv");
            foundMedia.ElementAt(1).Name.Should().Be("ShowName.Us.S01E01.720p.HDTV-GROUP.mkv");
        }

        [Test]
        public void IfNoMediaExistsReturnAnEmptyList()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\tmp.txt", new MockFileData("")},
            });
            var mediaFinder = new MediaFinder(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E01").ToList();

            foundMedia.Should().BeEmpty();
        }

        [Test]
        public void IfTheRootPathIsNotFoundThrowAnException()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            Action act = () => new MediaFinder(@"c:\video", fileSystem);

            act.ShouldThrow<ArgumentException>().WithMessage(@"Media path not found: c:\video");
        }
    }
}