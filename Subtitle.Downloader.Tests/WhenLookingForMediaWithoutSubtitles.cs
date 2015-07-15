namespace Subtitle.Downloader.Tests
{
    using System.Collections.Generic;
    using System.IO.Abstractions.TestingHelpers;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenLookingForMediaWithoutSubtitles
    {
        [Test]
        public void IfShowAndEpisodeExistWithoutASubReturnTheMedia()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")}
            });
            var mediaFinder = new MediaFinderWithoutSubtitles(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E01").ToList();

            foundMedia.Count().Should().Be(1);
            foundMedia.First().Name.Should().Be("ShowName.S01E01.720p.HDTV-GROUP.mkv");
        }

        [Test]
        public void IfShowAndEpisodeExistWithASubReturnNothing()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.mkv", new MockFileData("")},
                {@"c:\video\ShowName\S01E01\ShowName.S01E01.720p.HDTV-GROUP.srt", new MockFileData("")}
            });
            var mediaFinder = new MediaFinderWithoutSubtitles(@"c:\video", fileSystem);
            var foundMedia = mediaFinder.LookFor("ShowName", "S01E01").ToList();

            foundMedia.Should().BeEmpty();
        }
    }
}