namespace Subtitle.Provider.Addic7ed.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenInspectingASubtitlePage
    {
        private EpisodePage episodePage;

        [TestInitialize]
        public void Setup()
        {
            var reader = ResourceManager.GetInputFile("The Walking Dead - 04x08 - Too Far Gone.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [TestMethod]
        public void EpisodeSeasonShouldBeS04E08()
        {
            episodePage.SeasonEpisode.Should().Be("S04E08");
        }

        [TestMethod]
        public void IfThePageHasNoContentAnExceptionShouldBeThrown()
        {
            Action act = () => AddictedEpisodePageParser.For("");

            act.ShouldThrow<EpisodePageIsEmtpyException>().WithMessage("Page is empty");
        }

        [TestMethod]
        public void IfThePageHasNoTitleAnExceptionShouldBeThrown()
        {
            Action act = () => AddictedEpisodePageParser.For("<html><body></body></html>");

            act.ShouldThrow<ParsingException>().WithMessage("No title was found on the page");
        }
    }
}