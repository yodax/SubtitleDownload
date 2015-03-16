namespace Subtitle.Downloader.Tests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Provider.Addic7ed;

    [TestClass]
    public class WhenCreatingASeasonEpisodeString
    {
        [TestMethod]
        public void IfSeasonAndEpisodeBothAreBelowTenALeadingZeroShouldBeAdded()
        {
            ExtensionMethods.GenerateSeasonEpisode(1, 1).Should().Be("S01E01");
        }

        [TestMethod]
        public void IfSeasonAndEpisodeBotherAreGreatherThenTenNoLeadingZeroShouldBeAdded()
        {
            ExtensionMethods.GenerateSeasonEpisode(10, 10).Should().Be("S10E10");
        }
    }
}