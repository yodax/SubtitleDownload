namespace Subtitle.Downloader.Tests
{
    using FluentAssertions;
    using NUnit.Framework;
    using Provider.Addic7ed;

    [TestFixture]
    public class WhenCreatingASeasonEpisodeString
    {
        [Test]
        public void IfSeasonAndEpisodeBothAreBelowTenALeadingZeroShouldBeAdded()
        {
            ExtensionMethods.GenerateSeasonEpisode(1, 1).Should().Be("S01E01");
        }

        [Test]
        public void IfSeasonAndEpisodeBotherAreGreatherThenTenNoLeadingZeroShouldBeAdded()
        {
            ExtensionMethods.GenerateSeasonEpisode(10, 10).Should().Be("S10E10");
        }
    }
}