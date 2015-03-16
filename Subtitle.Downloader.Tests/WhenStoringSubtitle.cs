namespace Subtitle.Downloader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions.TestingHelpers;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenStoringSubtitle
    {
        [TestMethod]
        public void IfSrtContainsUnicodeItShouldBeStored()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\temp.txt", new MockFileData("")}
            });

            SubtitleDownloader.SaveStringToFile(mockFileSystem, @"c:\temp.srt", "Guantánamo");

            var content = mockFileSystem.File.ReadAllText(@"c:\temp.srt");
            content.Trim().Equals("Guantánamo", StringComparison.InvariantCulture).Should().BeTrue();
        }
    }
}