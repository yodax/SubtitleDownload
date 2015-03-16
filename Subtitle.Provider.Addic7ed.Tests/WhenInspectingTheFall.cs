namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenInspectingTheFall
    {
        private EpisodePage episodePage;

        [TestInitialize]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("The Fall - 02x06.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [TestMethod]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("The Fall");
        }

        [TestMethod]
        public void FirstVersionShouldBeUploadedByElderman()
        {
            episodePage.SubtitleVersions.First().Uploader.Should().Be("chamallow");
            episodePage.SubtitleVersions.Should().HaveCount(18);
        }

        [TestMethod]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(6);
            episodePage.Season.Should().Be(2);
            episodePage.EpisodeName.Should().Be("In Summation");
        }
    }
}