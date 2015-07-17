using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenInspectingTheWalkingDead201
    {
        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("The Walking Dead - 02x01.html");
            var pageContent = reader.ReadToEnd();

            _episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        private EpisodePage _episodePage;

        [Test]
        public void LookForTheEpisode()
        {
            _episodePage.Episode.Should().Be(1);
            _episodePage.Season.Should().Be(2);
            _episodePage.EpisodeName.Should().Be("What Lies Ahead");
        }

        [Test]
        public void LookForTheShowName()
        {
            _episodePage.ShowName.Should().Be("The Walking Dead");
        }
    }
}