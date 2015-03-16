namespace Subtitle.Provider.Addic7ed.Tests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenSearchingForShowLinks
    {
        [TestMethod]
        public void FindAllLinksForShow()
        {
            var pageContent = ResourceManager.GetInputFile("Search_Brickleberry.html").ReadToEnd();

            var searchPage = AddictedSearchPageParser.For(pageContent);

            searchPage.FoundLinks.Count().Should().Be(23);
        }

        [TestMethod]
        public void FindEpisodeInformationCorrectlty()
        {
            var pageContent = ResourceManager.GetInputFile("Search_Brickleberry.html").ReadToEnd();

            var searchPage = AddictedSearchPageParser.For(pageContent);

            var firstShow = searchPage.FoundLinks.First();
            firstShow.ShowName.Should().Be("Brickleberry");
            firstShow.SeasonEpisode.Should().Be("S01E01");
            firstShow.Link.Should().Be("http://www.addic7ed.com/serie/Brickleberry/1/1/Welcome_to_Brickleberry");
            firstShow.IgnoreAge.Should().BeTrue();

            var lastShow = searchPage.FoundLinks.ElementAt(22);
            lastShow.ShowName.Should().Be("Brickleberry");
            lastShow.SeasonEpisode.Should().Be("S02E13");
            lastShow.Link.Should().Be("http://www.addic7ed.com/serie/Brickleberry/2/13/A-Park-a-Lypse");
        }

        [TestMethod]
        public void IfThePageHasNoContentAnExceptionShouldBeThrown()
        {
            Action act = () => AddictedSearchPageParser.For("");

            act.ShouldThrow<ArgumentException>().WithMessage("Page is empty\r\nParameternaam: pageContent");
        }

        [TestMethod]
        public void ConstructPropperSearchUrl()
        {
            var url = "Showname With Spaces".ToSearchUrl();
            url.Should().Be(@"http://www.addic7ed.com/search.php?search=Showname+With+Spaces&Submit=Search");
        }
    }
}