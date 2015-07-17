using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenInspectingHawaiiFiveO
    {
        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Hawaii Five-0 (2010) - 04x10.html");
            var pageContent = reader.ReadToEnd();

            _episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        private EpisodePage _episodePage;

        [Test]
        public void FirstVersionShouldBeUploadedByElderman()
        {
            _episodePage.SubtitleVersions.First().Uploader.Should().Be("elderman");
        }

        [Test]
        public void LookForTheEpisode()
        {
            _episodePage.Episode.Should().Be(10);
            _episodePage.Season.Should().Be(4);
            _episodePage.EpisodeName.Should().Be("Ho'onani Makuakane (Honor Thy Father)");
        }

        [Test]
        public void LookForTheShowName()
        {
            _episodePage.ShowName.Should().Be("Hawaii Five-0 (2010)");
        }
    }
}