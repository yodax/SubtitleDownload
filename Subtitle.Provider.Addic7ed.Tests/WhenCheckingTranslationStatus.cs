using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenCheckingTranslationStatus
    {
        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Hawaii Five-0 (2010) - 04x10.html");
            var pageContent = reader.ReadToEnd();

            _episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        private EpisodePage _episodePage;

        [Test]
        public void EnglishShouldBeCompleted()
        {
            _episodePage.SubtitleVersions.First().Subtitles.First().Completed.Should().BeTrue();
        }

        [Test]
        public void PortugueseShouldNotBeCompleted()
        {
            var portugueseVersion = _episodePage.SubtitleVersions.First()
                .Subtitles.FirstOrDefault(s => s.Language.Equals("Portuguese (Brazilian)"));

            portugueseVersion.Should().NotBeNull();
            portugueseVersion.Completed.Should().BeFalse();
        }
    }
}