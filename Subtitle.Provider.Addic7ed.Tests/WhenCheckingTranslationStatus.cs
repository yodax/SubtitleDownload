namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenCheckingTranslationStatus
    {
        private EpisodePage episodePage;

        [TestInitialize]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Hawaii Five-0 (2010) - 04x10.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }


        [TestMethod]
        public void EnglishShouldBeCompleted()
        {
            episodePage.SubtitleVersions.First().Subtitles.First().Completed.Should().BeTrue();
        }

        [TestMethod]
        public void PortugueseShouldNotBeCompleted()
        {
            var portugueseVersion = episodePage.SubtitleVersions.First()
                .Subtitles.FirstOrDefault(s => s.Language.Equals("Portuguese (Brazilian)"));

            portugueseVersion.Should().NotBeNull();
            portugueseVersion.Completed.Should().BeFalse();
        }
    }
}