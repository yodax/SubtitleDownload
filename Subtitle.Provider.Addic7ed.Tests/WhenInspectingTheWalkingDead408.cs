using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenInspectingTheWalkingDead408
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
        public void FindTheDowloadLinks()
        {
            var firstVersion = _episodePage.SubtitleVersions.First();
            var frenchWithJustADownloadLink = firstVersion.Subtitles.First(s => s.Language.Equals("French"));

            frenchWithJustADownloadLink.Links.Count.Should().Be(1);
            frenchWithJustADownloadLink.Links.ElementAt(0).Type.Should().Be(SubtitleLinkType.Download);
            frenchWithJustADownloadLink.Links.ElementAt(0).Link.Should().Be("http://www.addic7ed.com/updated/8/81967/0");
        }

        [Test]
        public void FindTheDowloadLinksWithAnOriginalAndUpdatedVersion()
        {
            var firstVersion = _episodePage.SubtitleVersions.First();
            var englishUpdatedLanguage = firstVersion.Subtitles.First(s => s.Language.Equals("English"));

            englishUpdatedLanguage.Links.Count.Should().Be(2);
            var originalLink = englishUpdatedLanguage.Links.ElementAt(0);
            originalLink.Type.Should().Be(SubtitleLinkType.Download);
            originalLink.Link.Should().Be("http://www.addic7ed.com/original/81967/0");
            var updatedLink = englishUpdatedLanguage.Links.ElementAt(1);
            updatedLink.Type.Should().Be(SubtitleLinkType.Updated);
            updatedLink.Link.Should().Be("http://www.addic7ed.com/updated/1/81967/0");
        }

        [Test]
        public void FindTheSubtitleVersions()
        {
            _episodePage.SubtitleVersions.Count().Should().Be(21);
        }

        [Test]
        public void IfTheReleaseHasExtraGroupsTheShouldBeAddedToTheReleaseInformation()
        {
            var versionWithExtraInfo = _episodePage.SubtitleVersions.ElementAt(2);
            versionWithExtraInfo.Release.Should().Be("KILLERS Works with AFG, HDTV mSD");
        }

        [Test]
        public void LookForTheEpisode()
        {
            _episodePage.Episode.Should().Be(8);
            _episodePage.Season.Should().Be(4);
            _episodePage.EpisodeName.Should().Be("Too Far Gone");
        }

        [Test]
        public void LookForTheShowName()
        {
            _episodePage.ShowName.Should().Be("The Walking Dead");
        }
    }
}