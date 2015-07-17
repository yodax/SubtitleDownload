using System;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Provider.Addic7ed.Tests
{
    [TestFixture]
    public class WhenInspectingASubtitlePageButYouGetAnIndex
    {
        [Test]
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