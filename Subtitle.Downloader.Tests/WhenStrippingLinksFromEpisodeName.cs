namespace Subtitle.Downloader.Tests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Provider.Addic7ed;

    [TestClass]
    public class WhenStrippingLinksFromEpisodeName
    {
        [TestMethod]
        public void TheEpisodeShouldBeStripped()
        {
            @"http://www.addic7ed.com/serie/Anger_Management/2/43/Charlie_Loses_His_Virginity_Again"
                .StripAddictedEpisode()
                .Should()
                .Be(@"http://www.addic7ed.com/serie/Anger_Management/2/43/");
        }
    }
}