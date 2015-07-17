using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenInspectingTheFall
    {
        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("The Fall - 02x06.html");
            var pageContent = reader.ReadToEnd();

            _episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        private EpisodePage _episodePage;

        [Test]
        public void FirstVersionShouldBeUploadedByElderman()
        {
            _episodePage.SubtitleVersions.First().Uploader.Should().Be("chamallow");
            _episodePage.SubtitleVersions.Should().HaveCount(18);
        }

        [Test]
        public void LookForTheEpisode()
        {
            _episodePage.Episode.Should().Be(6);
            _episodePage.Season.Should().Be(2);
            _episodePage.EpisodeName.Should().Be("In Summation");
        }

        [Test]
        public void LookForTheShowName()
        {
            _episodePage.ShowName.Should().Be("The Fall");
        }
    }
}