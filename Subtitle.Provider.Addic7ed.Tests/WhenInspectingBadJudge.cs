using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenInspectingBadJudge
    {
        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("Bad Judge - 01x12.html");
            var pageContent = reader.ReadToEnd();

            _episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        private EpisodePage _episodePage;

        [Test]
        public void FirstVersionShouldBeUploadedByKateGreen()
        {
            _episodePage.SubtitleVersions.First().Uploader.Should().Be("kategreen");
            _episodePage.SubtitleVersions.Should().HaveCount(2);
        }

        [Test]
        public void LookForTheEpisode()
        {
            _episodePage.Episode.Should().Be(12);
            _episodePage.Season.Should().Be(1);
            _episodePage.EpisodeName.Should().Be("Lockdown");
        }

        [Test]
        public void LookForTheShowName()
        {
            _episodePage.ShowName.Should().Be("Bad Judge");
        }
    }
}