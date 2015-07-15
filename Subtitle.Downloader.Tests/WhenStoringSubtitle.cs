namespace Subtitle.Downloader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions.TestingHelpers;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenStoringSubtitle
    {
        [Test]
        public void IfSrtContainsUnicodeItShouldBeStored()
        {
            var mockFileSystem = new MockFileSystem();

            SubtitleDownloader.SaveStringToFile(mockFileSystem, @"c:\temp.srt", "Guantánamo");

            var content = mockFileSystem.File.ReadAllText(@"c:\temp.srt");
            content.Trim().Equals("Guantánamo", StringComparison.InvariantCulture).Should().BeTrue();
        }
    }
}