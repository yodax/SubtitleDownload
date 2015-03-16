namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenInspectingTheWalkingDead408
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
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("The Walking Dead");
        }

        [TestMethod]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(8);
            episodePage.Season.Should().Be(4);
            episodePage.EpisodeName.Should().Be("Too Far Gone");
        }

        [TestMethod]
        public void FindTheSubtitleVersions()
        {
            episodePage.SubtitleVersions.Count().Should().Be(21);
        }

        [TestMethod]
        public void IfTheReleaseHasExtraGroupsTheShouldBeAddedToTheReleaseInformation()
        {
            var versionWithExtraInfo = episodePage.SubtitleVersions.ElementAt(2);
            versionWithExtraInfo.Release.Should().Be("KILLERS Works with AFG, HDTV mSD");
        }

        [TestMethod]
        public void FindTheDowloadLinksWithAnOriginalAndUpdatedVersion()
        {
            var firstVersion = episodePage.SubtitleVersions.First();
            var englishUpdatedLanguage = firstVersion.Subtitles.First(s => s.Language.Equals("English"));

            englishUpdatedLanguage.Links.Count.Should().Be(2);
            var originalLink = englishUpdatedLanguage.Links.ElementAt(0);
            originalLink.Type.Should().Be(SubtitleLinkType.Download);
            originalLink.Link.Should().Be("http://www.addic7ed.com/original/81967/0");
            var updatedLink = englishUpdatedLanguage.Links.ElementAt(1);
            updatedLink.Type.Should().Be(SubtitleLinkType.Updated);
            updatedLink.Link.Should().Be("http://www.addic7ed.com/updated/1/81967/0");
        }

        [TestMethod]
        public void FindTheDowloadLinks()
        {
            var firstVersion = episodePage.SubtitleVersions.First();
            var frenchWithJustADownloadLink = firstVersion.Subtitles.First(s => s.Language.Equals("French"));

            frenchWithJustADownloadLink.Links.Count.Should().Be(1);
            frenchWithJustADownloadLink.Links.ElementAt(0).Type.Should().Be(SubtitleLinkType.Download);
            frenchWithJustADownloadLink.Links.ElementAt(0).Link.Should().Be("http://www.addic7ed.com/updated/8/81967/0");
        }
    }
}