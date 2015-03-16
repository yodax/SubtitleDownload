namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenInspectingAngerManagement
    {
        private EpisodePage episodePage;

        [TestInitialize]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Anger Management - 02x43 - Charlie Loses His Virginity Again.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [TestMethod]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("Anger Management");
        }

        [TestMethod]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(43);
            episodePage.Season.Should().Be(2);
            episodePage.EpisodeName.Should().Be("Charlie Loses His Virginity Again");
        }

        [TestMethod]
        public void FindTheSubtitleVersions()
        {
            episodePage.SubtitleVersions.Count().Should().Be(9);
        }

        [TestMethod]
        public void FindTheSubtitles()
        {
            var firstVersion = episodePage.SubtitleVersions.First();

            firstVersion.Subtitles.Count.Should().Be(3);
        }

        [TestMethod]
        public void FindTheSubtitleLanguage()
        {
            var dutchVersion = episodePage.SubtitleVersions.Last();

            dutchVersion.Subtitles[0].Language.Should().Be("Dutch");
        }

        [TestMethod]
        public void FindTheVersionAge()
        {
            var versionWithDays = episodePage.SubtitleVersions.First();

            versionWithDays.Age.Days.Should().Be(12);

            var versionWithHours = episodePage.SubtitleVersions.ElementAt(1);

            versionWithHours.Age.Hours.Should().Be(12);
        }

        [TestMethod]
        public void HearingImpairedSubtitleShouldBeMarked()
        {
            var nonImpairedVersion = episodePage.SubtitleVersions.ElementAt(0);
            var impairedVersion = episodePage.SubtitleVersions.ElementAt(1);

            nonImpairedVersion.Subtitles[0].HearingImpaired.Should().BeFalse();
            impairedVersion.Subtitles[0].HearingImpaired.Should().BeTrue();
        }

        [TestMethod]
        public void DownloadCountShouldBeStored()
        {
            var subtitleWithDownloads = episodePage.SubtitleVersions.ElementAt(0).Subtitles.ElementAt(0);

            subtitleWithDownloads.Downloads.Should().Be(4610);
        }

        [TestMethod]
        public void IfNotDownloadedYetDownloadCountShouldBeZero()
        {
            var subtitleWithDownloads = episodePage.SubtitleVersions.ElementAt(0).Subtitles.ElementAt(1);

            subtitleWithDownloads.Downloads.Should().Be(0);
        }

        [TestMethod]
        public void FindMultipleSubtitleLanguages()
        {
            var firstVersion = episodePage.SubtitleVersions.First();

            firstVersion.Subtitles[0].Language.Should().Be("English");
            firstVersion.Subtitles[1].Language.Should().Be("Bulgarian");
            firstVersion.Subtitles[2].Language.Should().Be("French");
        }

        [TestMethod]
        public void FindTheSubtitleVersionInformation()
        {
            episodePage.SubtitleVersions.ElementAt(0).Release.Should().Be("x264-KILLERS");
            episodePage.SubtitleVersions.ElementAt(1).Release.Should().Be("x264-KILLERS");
            episodePage.SubtitleVersions.ElementAt(2).Release.Should().StartWith("KILLERS");
            episodePage.SubtitleVersions.ElementAt(3).Release.Should().Be("HDTVx264-2HD");
            episodePage.SubtitleVersions.ElementAt(4).Release.Should().Be("KILLERS");
            episodePage.SubtitleVersions.ElementAt(5).Release.Should().Be("720p Web-DL");
            episodePage.SubtitleVersions.ElementAt(6).Release.Should().Be("KILLERS");
            episodePage.SubtitleVersions.ElementAt(7).Release.Should().StartWith("KILLERS");
            episodePage.SubtitleVersions.ElementAt(8).Release.Should().Be("WEB-DL");
        }
    }
}