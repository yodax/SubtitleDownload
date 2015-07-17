using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenInspectingCsi
    {
        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("CSI - 15x09.html");
            var pageContent = reader.ReadToEnd();

            _episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        private EpisodePage _episodePage;

        [Test]
        public void FirstVersionShouldBeUploadedByElderman()
        {
            _episodePage.SubtitleVersions.First().Uploader.Should().Be("elderman");
            _episodePage.SubtitleVersions.Should().HaveCount(4);
        }

        [Test]
        public void LookForTheEpisode()
        {
            _episodePage.Episode.Should().Be(9);
            _episodePage.Season.Should().Be(15);
            _episodePage.EpisodeName.Should().Be("Let's Make a Deal");
        }

        [Test]
        public void LookForTheShowName()
        {
            _episodePage.ShowName.Should().Be("CSI: Crime Scene Investigation");
        }
    }
}