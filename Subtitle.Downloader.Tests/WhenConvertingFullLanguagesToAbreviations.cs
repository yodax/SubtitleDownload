using FluentAssertions;
using NUnit.Framework;



namespace Subtitle.Downloader.Tests
{
    [TestFixture]
    public class WhenConvertingFullLanguagesToAbreviations
    {
        [Test]
        public void DutchShouldBeConvertedToNl()
        {
            "Dutch".ToShortLanguage().Should().Be("nl");
        }

        [Test]
        public void EnglishShouldBeConvertedToEn()
        {
            "English".ToShortLanguage().Should().Be("en");
        }
    }
}