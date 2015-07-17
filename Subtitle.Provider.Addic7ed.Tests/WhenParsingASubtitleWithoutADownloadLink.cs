using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenParsingASubtitleWithoutADownloadLink
    {
        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Supernatural - 09x03.html");
            var pageContent = reader.ReadToEnd();

            _episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        private EpisodePage _episodePage;

        [Test]
        public void LookForTheShowName()
        {
            _episodePage.ShowName.Should().Be("Supernatural");
        }

        [Test]
        public void TheSecondToLastVersionShouldHaveNoSubtitles()
        {
            _episodePage.SubtitleVersions.Should().HaveCount(13);
        }
    }
}