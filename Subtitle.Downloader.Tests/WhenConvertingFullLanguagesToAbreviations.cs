using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace Subtitle.Downloader.Tests
{
    [TestClass]
    public class WhenConvertingFullLanguagesToAbreviations
    {
        [TestMethod]
        public void DutchShouldBeConvertedToNl()
        {
            "Dutch".ToShortLanguage().Should().Be("nl");
        }

        [TestMethod]
        public void EnglishShouldBeConvertedToEn()
        {
            "English".ToShortLanguage().Should().Be("en");
        }
    }
}