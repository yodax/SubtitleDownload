namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenInspectingHawaiiFiveO
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
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("Hawaii Five-0 (2010)");
        }

        [TestMethod]
        public void FirstVersionShouldBeUploadedByElderman()
        {
            episodePage.SubtitleVersions.First().Uploader.Should().Be("elderman");
        }

        [TestMethod]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(10);
            episodePage.Season.Should().Be(4);
            episodePage.EpisodeName.Should().Be("Ho'onani Makuakane (Honor Thy Father)");
        }
    }
}