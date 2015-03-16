namespace Subtitle.Provider.Addic7ed.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenInspectingASubtitlePageButYouGetAnIndex
    {
        [TestMethod]
        public void ShouldThrowAnEpisodePageNoLongerExists()
        {
            var reader =
                ResourceManager.GetInputFile("Index page.html");
            var pageContent = reader.ReadToEnd();

            Action action = () => AddictedEpisodePageParser.For(pageContent);

            action.ShouldThrow<EpisodePageNoLongerExists>().WithMessage("Episode page no longer exists");
        }
    }
}