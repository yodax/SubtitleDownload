namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenInspectingCsi
    {
        private EpisodePage episodePage;

        [TestInitialize]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("CSI - 15x09.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [TestMethod]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("CSI: Crime Scene Investigation");
        }

        [TestMethod]
        public void FirstVersionShouldBeUploadedByElderman()
        {
            episodePage.SubtitleVersions.First().Uploader.Should().Be("elderman");
            episodePage.SubtitleVersions.Should().HaveCount(4);
        }

        [TestMethod]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(9);
            episodePage.Season.Should().Be(15);
            episodePage.EpisodeName.Should().Be("Let's Make a Deal");
        }
    }
}