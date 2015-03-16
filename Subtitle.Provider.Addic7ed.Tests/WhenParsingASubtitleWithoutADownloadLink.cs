namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenParsingASubtitleWithoutADownloadLink
    {
        private EpisodePage episodePage;

        [TestInitialize]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Supernatural - 09x03.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [TestMethod]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("Supernatural");
        }

        [TestMethod]
        public void TheSecondToLastVersionShouldHaveNoSubtitles()
        {
            episodePage.SubtitleVersions.Should().HaveCount(13);
        }
    }
}