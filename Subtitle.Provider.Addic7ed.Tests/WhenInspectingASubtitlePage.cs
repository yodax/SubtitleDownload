using System;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenInspectingASubtitlePage
    {
        [SetUp]
        public void Setup()
        {
            var reader = ResourceManager.GetInputFile("The Walking Dead - 04x08 - Too Far Gone.html");
            var pageContent = reader.ReadToEnd();

            _episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        private EpisodePage _episodePage;

        [Test]
        public void EpisodeSeasonShouldBeS04E08()
        {
            _episodePage.SeasonEpisode.Should().Be("S04E08");
        }

        [Test]
        public void IfThePageHasNoContentAnExceptionShouldBeThrown()
        {
            Action act = () => AddictedEpisodePageParser.For("");

            act.ShouldThrow<EpisodePageIsEmtpyException>().WithMessage("Page is empty");
        }

        [Test]
        public void IfThePageHasNoTitleAnExceptionShouldBeThrown()
        {
            Action act = () => AddictedEpisodePageParser.For("<html><body></body></html>");

            act.ShouldThrow<ParsingException>().WithMessage("No title was found on the page");
        }
    }
}