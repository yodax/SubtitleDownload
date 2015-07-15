namespace Subtitle.Provider.Addic7ed.Tests
{
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenInspectingTheWalkingDead201
    {
        private EpisodePage episodePage;

        [SetUp]
        public void Setup()
        {
            var reader =
                ResourceManager.GetInputFile("The Walking Dead - 02x01.html");
            var pageContent = reader.ReadToEnd();

            episodePage = AddictedEpisodePageParser.For(pageContent);
        }

        [Test]
        public void LookForTheShowName()
        {
            episodePage.ShowName.Should().Be("The Walking Dead");
        }

        [Test]
        public void LookForTheEpisode()
        {
            episodePage.Episode.Should().Be(1);
            episodePage.Season.Should().Be(2);
            episodePage.EpisodeName.Should().Be("What Lies Ahead");
        }
    }
}