using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenExtractingLinksFromAddictedFeeds
    {
        [Test]
        public void IfTheFeedHasErrors()
        {
            var feedContentReader = ResourceManager.GetInputFile("FaultyRss.xml");
            var feedContent = feedContentReader.ReadToEnd();

            var links = AddictedFeedReader.GetAllLinksFrom(feedContent).ToList();

            links.Count().Should().Be(15);
            //links.First().Should().Be("http://www.addic7ed.com/serie/Marvel%27s_Agents_of_S.H.I.E.L.D./1/9/Repairs14");
            //links.ElementAt(14).Should().Be("http://www.addic7ed.com/serie/Brooklyn_Nine-Nine/1/11/Christmas1");
        }

        [Test]
        public void IfTheFeedIsHotspotAllLinksShouldBeExtracted()
        {
            var feedContentReader = ResourceManager.GetInputFile("AddictedHotspot.xml");
            var feedContent = feedContentReader.ReadToEnd();

            var links = AddictedFeedReader.GetAllLinksFrom(feedContent).ToList();

            links.Count().Should().Be(15);
            links.First().Should().Be("http://www.addic7ed.com/serie/Sons_of_Anarchy/6/12/You_Are_My_Sunshine");
            links.ElementAt(14).Should().Be("http://www.addic7ed.com/serie/Hebburn/2/4/Stairway_to_Hebburn");
        }

        [Test]
        public void IfTheFeedIsLastNewVersionAllLinksShouldBeExtracted()
        {
            var feedContentReader = ResourceManager.GetInputFile("Addic7edLastNewVersions.xml");
            var feedContent = feedContentReader.ReadToEnd();

            var links = AddictedFeedReader.GetAllLinksFrom(feedContent).ToList();

            links.Count().Should().Be(15);
            links.First().Should().Be("http://www.addic7ed.com/serie/Marvel%27s_Agents_of_S.H.I.E.L.D./1/9/Repairs14");
            links.ElementAt(14).Should().Be("http://www.addic7ed.com/serie/Brooklyn_Nine-Nine/1/11/Christmas1");
        }

        [Test]
        public void IfTheFeedIsLastUploadedAllLinksShouldBeExtracted()
        {
            var feedContentReader = ResourceManager.GetInputFile("Addic7edLastUploaded.xml");
            var feedContent = feedContentReader.ReadToEnd();

            var links = AddictedFeedReader.GetAllLinksFrom(feedContent).ToList();

            links.Count().Should().Be(15);
            links.First().Should().Be("http://www.addic7ed.com/serie/Marvel%27s_Agents_of_S.H.I.E.L.D./1/9/Repairs");
            links.ElementAt(14).Should().Be("http://www.addic7ed.com/serie/The_Blacklist/1/10/Anslo_Garrick,_Part_2");
        }
    }
}