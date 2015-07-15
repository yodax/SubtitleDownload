namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenInspectingCsi
    {
        private EpisodePage episodePage;

        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("CSI - 15x09.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [Test]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("CSI: Crime Scene Investigation");
        }

        [Test]
        public void FirstVersionShouldBeUploadedByElderman()
        {
            episodePage.SubtitleVersions.First().Uploader.Should().Be("elderman");
            episodePage.SubtitleVersions.Should().HaveCount(4);
        }

        [Test]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(9);
            episodePage.Season.Should().Be(15);
            episodePage.EpisodeName.Should().Be("Let's Make a Deal");
        }
    }
}