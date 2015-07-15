namespace Subtitle.Downloader.Tests
{
    using FluentAssertions;
    using NUnit.Framework;
    using Provider.Addic7ed;

    [TestFixture]
    public class WhenStrippingLinksFromEpisodeName
    {
        [Test]
        public void TheEpisodeShouldBeStripped()
        {
            @"http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                .StripAddictedEpisode()
                .Should()
                .Be(@"http://www.addic7ed.com/serie/Anger_Management/2/43/");
        }
    }
}