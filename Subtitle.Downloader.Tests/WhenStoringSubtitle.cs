using System;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Downloader.Tests
{
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