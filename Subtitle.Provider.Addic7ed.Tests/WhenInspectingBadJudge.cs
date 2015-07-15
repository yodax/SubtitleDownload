namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenInspectingBadJudge
    {
        private EpisodePage episodePage;

        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Bad Judge - 01x12.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [Test]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("Bad Judge");
        }

        [Test]
        public void FirstVersionShouldBeUploadedByKateGreen()
        {
            episodePage.SubtitleVersions.First().Uploader.Should().Be("kategreen");
            episodePage.SubtitleVersions.Should().HaveCount(2);
        }

        [Test]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(12);
            episodePage.Season.Should().Be(1);
            episodePage.EpisodeName.Should().Be("Lockdown");
        }
    }
}