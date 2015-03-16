namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenInspectingBadJudge
    {
        private EpisodePage episodePage;

        [TestInitialize]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Bad Judge - 01x12.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [TestMethod]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("Bad Judge");
        }

        [TestMethod]
        public void FirstVersionShouldBeUploadedByKateGreen()
        {
            episodePage.SubtitleVersions.First().Uploader.Should().Be("kategreen");
            episodePage.SubtitleVersions.Should().HaveCount(2);
        }

        [TestMethod]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(12);
            episodePage.Season.Should().Be(1);
            episodePage.EpisodeName.Should().Be("Lockdown");
        }
    }
}