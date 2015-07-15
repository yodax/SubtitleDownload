namespace Subtitle.Downloader.Tests
{
    using FluentAssertions;
    using NUnit.Framework;
    using Provider.Addic7ed;

    [TestFixture]
    public class WhenTransFormingAFoundLinkToEpisodeInfo
    {
        private static EpisodeInfo episodeInfo;

        [Test]
        public void TheEpisodeInfoShouldContainTheShowName()
        {
            GivenALink("http://www.addic7ed.com/serie/Ripper_Street/2/7/Our_Betrayal_(1)");
            ThenTheShowNameShouldBe("Ripper Street");
        }

        [Test]
        public void IfTheShowNameContainsHtmlEscapeDecodeThese()
        {
            GivenALink("http://www.addic7ed.com/serie/Atlantis%3A_%282013%29/1/10/The_Price_of_Hope");
            ThenTheShowNameShouldBe("Atlantis: (2013)");
        }

        [Test]
        public void TheEpisodeInfoShouldContainTheSeasonEpisodeString()
        {
            GivenALink("http://www.addic7ed.com/serie/Ripper_Street/2/7/Our_Betrayal_(1)");
            ThenTheShowNameShouldBe("Ripper Street");
            ThenTheEpisodeShouldBe("S02E07");
        }

        [Test]
        public void IfItIsASearchStringTheEpisodeInfoShouldContainTheShowInfo()
        {
            GivenALink("http://www.addic7ed.com/search.php?search=Anger+Management+2x43&Submit=Search");
            ThenTheShowNameShouldBe("Anger Management");
            ThenTheEpisodeShouldBe("S02E43");
        }

        private static void ThenTheEpisodeShouldBe(string episode)
        {
            episodeInfo.SeasonEpisode.Should().Be(episode);
        }

        private static void ThenTheShowNameShouldBe(string showName)
        {
            episodeInfo.ShowName.Should().Be(showName);
        }

        private static void GivenALink(string link)
        {
            var foundLink = new FoundLink
            {
                Link = link
            };

            episodeInfo = foundLink.GetEpisodeInfo();
        }
    }
}