namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenParsingASubtitleWithoutADownloadLink
    {
        private EpisodePage episodePage;

        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Supernatural - 09x03.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [Test]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("Supernatural");
        }

        [Test]
        public void TheSecondToLastVersionShouldHaveNoSubtitles()
        {
            episodePage.SubtitleVersions.Should().HaveCount(13);
        }
    }
}