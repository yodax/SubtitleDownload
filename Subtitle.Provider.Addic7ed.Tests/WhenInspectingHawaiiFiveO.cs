namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenInspectingHawaiiFiveO
    {
        private EpisodePage episodePage;

        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Hawaii Five-0 (2010) - 04x10.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [Test]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("Hawaii Five-0 (2010)");
        }

        [Test]
        public void FirstVersionShouldBeUploadedByElderman()
        {
            episodePage.SubtitleVersions.First().Uploader.Should().Be("elderman");
        }

        [Test]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(10);
            episodePage.Season.Should().Be(4);
            episodePage.EpisodeName.Should().Be("Ho'onani Makuakane (Honor Thy Father)");
        }
    }
}