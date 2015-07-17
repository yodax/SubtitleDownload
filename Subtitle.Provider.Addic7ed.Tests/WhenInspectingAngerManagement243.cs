using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenInspectingAngerManagement
    {
        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Anger Management - 02x43 - Charlie Loses His Virginity Again.html");
            var pageContent = reader.ReadToEnd();

            _episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        private EpisodePage _episodePage;

        [Test]
        public void DownloadCountShouldBeStored()
        {
            var subtitleWithDownloads = _episodePage.SubtitleVersions.ElementAt(0).Subtitles.ElementAt(0);

            subtitleWithDownloads.Downloads.Should().Be(4610);
        }

        [Test]
        public void FindMultipleSubtitleLanguages()
        {
            var firstVersion = _episodePage.SubtitleVersions.First();

            firstVersion.Subtitles[0].Language.Should().Be("English");
            firstVersion.Subtitles[1].Language.Should().Be("Bulgarian");
            firstVersion.Subtitles[2].Language.Should().Be("French");
        }

        [Test]
        public void FindTheSubtitleLanguage()
        {
            var dutchVersion = _episodePage.SubtitleVersions.Last();

            dutchVersion.Subtitles[0].Language.Should().Be("Dutch");
        }

        [Test]
        public void FindTheSubtitles()
        {
            var firstVersion = _episodePage.SubtitleVersions.First();

            firstVersion.Subtitles.Count.Should().Be(3);
        }

        [Test]
        public void FindTheSubtitleVersionInformation()
        {
            _episodePage.SubtitleVersions.ElementAt(0).Release.Should().Be("x264-KILLERS");
            _episodePage.SubtitleVersions.ElementAt(1).Release.Should().Be("x264-KILLERS");
            _episodePage.SubtitleVersions.ElementAt(2).Release.Should().StartWith("KILLERS");
            _episodePage.SubtitleVersions.ElementAt(3).Release.Should().Be("HDTVx264-2HD");
            _episodePage.SubtitleVersions.ElementAt(4).Release.Should().Be("KILLERS");
            _episodePage.SubtitleVersions.ElementAt(5).Release.Should().Be("720p Web-DL");
            _episodePage.SubtitleVersions.ElementAt(6).Release.Should().Be("KILLERS");
            _episodePage.SubtitleVersions.ElementAt(7).Release.Should().StartWith("KILLERS");
            _episodePage.SubtitleVersions.ElementAt(8).Release.Should().Be("WEB-DL");
        }

        [Test]
        public void FindTheSubtitleVersions()
        {
            _episodePage.SubtitleVersions.Count().Should().Be(9);
        }

        [Test]
        public void FindTheVersionAge()
        {
            var versionWithDays = _episodePage.SubtitleVersions.First();

            versionWithDays.Age.Days.Should().Be(12);

            var versionWithHours = _episodePage.SubtitleVersions.ElementAt(1);

            versionWithHours.Age.Hours.Should().Be(12);
        }

        [Test]
        public void HearingImpairedSubtitleShouldBeMarked()
        {
            var nonImpairedVersion = _episodePage.SubtitleVersions.ElementAt(0);
            var impairedVersion = _episodePage.SubtitleVersions.ElementAt(1);

            nonImpairedVersion.Subtitles[0].HearingImpaired.Should().BeFalse();
            impairedVersion.Subtitles[0].HearingImpaired.Should().BeTrue();
        }

        [Test]
        public void IfNotDownloadedYetDownloadCountShouldBeZero()
        {
            var subtitleWithDownloads = _episodePage.SubtitleVersions.ElementAt(0).Subtitles.ElementAt(1);

            subtitleWithDownloads.Downloads.Should().Be(0);
        }

        [Test]
        public void LookForTheEpisode()
        {
            _episodePage.Episode.Should().Be(43);
            _episodePage.Season.Should().Be(2);
            _episodePage.EpisodeName.Should().Be("Charlie Loses His Virginity Again");
        }

        [Test]
        public void LookForTheShowName()
        {
            _episodePage.ShowName.Should().Be("Anger Management");
        }
    }
}