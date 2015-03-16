namespace Subtitle.Downloader.Tests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenCheckingForAMatchingSubtitle
    {
        /*LOL & SYS always work with 720p DIMENSION;	
         * XII & ASAP always work with 720p IMMERSE; 
         * EXCELLENCE always works with REMARKABLE;
         * 2HD always works with 720p 2HD; 
         * BiA always works with 720p BiA; 
         * FoV always works with 720p FoV*/

        [TestMethod]
        public void IfSourceGroupIsEither2HdOrBiaOrFovThenMatchIgnoringResolution()
        {
            CheckWithDifferentResolutionsThatShouldMatch("2HD");
            CheckWithDifferentResolutionsThatShouldMatch("BIA");
            CheckWithDifferentResolutionsThatShouldMatch("FOV");
        }

        [TestMethod]
        public void IfSourceGroupIsEither2HdOrBiaOrFovThenOnlyMatchIfGroupIsInRelease()
        {
            CheckMatch.For("Blaat.S01E02.720p.HDTV-2HD.mkv", "HDTV-IMMERSE").Should().BeFalse();
        }

        private static void CheckWithDifferentResolutionsThatShouldMatch(string groupName)
        {
            var episode = "Blaat.S01E02.720p.HDTV-" + groupName + ".mkv";
            var subtitle = "HDTV-" + groupName;
            CheckMatch.For(episode, subtitle).Should().BeTrue();
        }

        [TestMethod]
        public void IfSourceGroupIsInterchangeAbleItShouldMatch()
        {
            CheckForInterChangeableGroups(new[]
            {
                "LOL",
                "SYS",
                "DIMENSION"
            });

            CheckForInterChangeableGroups(new[]
            {
                "XII",
                "ASAP",
                "IMMERSE"
            });

            CheckForInterChangeableGroups(new[]
            {
                "REMARKABLE",
                "EXCELLENCE"
            });
        }

        private static void CheckForInterChangeableGroups(string[] interchangeableGroups)
        {
            foreach (var releaseGroup in interchangeableGroups)
            {
                foreach (var subtitleGroup in interchangeableGroups)
                {
                    var episode = "Blaat.S01E02.HDTV-" + releaseGroup + ".mkv";
                    var subtitle = subtitleGroup;
                    CheckMatch.For(episode, subtitle).Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public void IfGroupIsKillersWithoutExtensionAndSourceIsOtherTheDontMatch()
        {
            SubtitleShouldNotMatchWithRelease("OTHER", "ShowName.S01E02.720p.HDTV-KILLERS");
        }

        private static void SubtitleShouldNotMatchWithRelease(string subtitleInformation, string releaseDescription)
        {
            CheckMatch.For(releaseDescription, subtitleInformation).Should().BeFalse();
        }

        private static void SubtitleShouldMatchWithRelease(string subtitleInformation, string releaseDescription)
        {
            CheckMatch.For(releaseDescription, subtitleInformation).Should().BeTrue();
        }

        [TestMethod]
        public void IfSourceGroupMatchesButNotResolutionThenDontMatch()
        {
            SubtitleShouldNotMatchWithRelease("480p.HDTV-GROUP", "Blaat.S01E02.720p.HDTV-GROUP.mkv");
        }

        [TestMethod]
        public void IfSourceGroupAndResolution720MatchesThenFindMatch()
        {
            SubtitleShouldMatchWithRelease("720p.HDTV-IMMERSE", "Blaat.S01E02.720p.HDTV-IMMERSE.mkv");
        }

        [TestMethod]
        public void IfSourceGroupAndResolution1080MatchesThenFindMatch()
        {
            SubtitleShouldMatchWithRelease("1080p.HDTV-IMMERSE", "Blaat.S01E02.1080p.HDTV-IMMERSE.mkv");
        }

        [TestMethod]
        public void IfSourceGroupAndResolution480MatchesThenFindMatch()
        {
            SubtitleShouldMatchWithRelease("480p.HDTV-IMMERSE", "Blaat.S01E02.480p.HDTV-IMMERSE.mkv");
        }

        [TestMethod]
        public void IfSourceGroupAndResolutionHdtvMatchesThenFindMatch()
        {
            SubtitleShouldMatchWithRelease("HDTV-IMMERSE", "Blaat.S01E02.HDTV-IMMERSE.mkv");
        }

        [TestMethod]
        public void IfSourceGroupMatchesButResolutionDoesntThenDontMatch()
        {
            SubtitleShouldNotMatchWithRelease("720p.HDTV-GROUP", "Blaat.S01E02.480p.HDTV-GROUP.mkv");
        }

        [TestMethod]
        public void IfSourceGroupMatchesButResolutionIsNotFoundThenMatch()
        {
            SubtitleShouldMatchWithRelease("IMMERSE", "Blaat.S01E02.720p.HDTV-IMMERSE.mkv");
        }

        [TestMethod]
        public void IfGroupContainsWebAndReleaseContainsWebThenMatch()
        {
            SubtitleShouldMatchWithRelease("WEB-DL", "Blaat.S01E02.WEB-DL-IMMERSE.mkv");
        }

        [TestMethod]
        public void IfGroupContainsWebAndRipAndReleaseContainsWebThenMatch()
        {
            SubtitleShouldMatchWithRelease("WEB", "Blaat.S01E02.WEBRip-IMMERSE.mkv");
        }

        [TestMethod]
        public void IfGroupsDontMatchDontMatchRelease()
        {
            SubtitleShouldNotMatchWithRelease("GROUP", "Blaat.S01E02.720p.HDTV-OTHERGROUP.mkv");
        }

        [TestMethod]
        public void IfReleaseContainsExtraGroupsTheyShouldMatch()
        {
            CheckMatch.For("Blaat.S01E02.720p.HDTV-IMMERSE.mkv", "ASAP works with IMMERSE").Should().BeTrue();
            SubtitleShouldMatchWithRelease("ASAP works with IMMERSE", "Blaat.S01E02.720p.HDTV-IMMERSE.mkv");
        }

        [TestMethod]
        public void IfReleaseContainsMultipleResolutionsAndThatIsContainedInTheReleaseTheyShouldMatch()
        {
            SubtitleShouldMatchWithRelease("ASAP works with IMMERSE 480p 720p", "Blaat.S01E02.720p.HDTV-IMMERSE.mkv");
        }
    }
}