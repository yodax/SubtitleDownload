using FluentAssertions;
using NUnit.Framework;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader.Tests
{
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