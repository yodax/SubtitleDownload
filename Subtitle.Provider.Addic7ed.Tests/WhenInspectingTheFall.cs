namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenInspectingTheFall
    {
        private EpisodePage episodePage;

        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("The Fall - 02x06.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [Test]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("The Fall");
        }

        [Test]
        public void FirstVersionShouldBeUploadedByElderman()
        {
            episodePage.SubtitleVersions.First().Uploader.Should().Be("chamallow");
            episodePage.SubtitleVersions.Should().HaveCount(18);
        }

        [Test]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(6);
            episodePage.Season.Should().Be(2);
            episodePage.EpisodeName.Should().Be("In Summation");
        }
    }
}