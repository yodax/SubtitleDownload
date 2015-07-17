using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Subtitle.Downloader
{
    public static class CheckMatch
    {
        public static bool For(string releaseName, string subtitleInformation)
        {
            var release = releaseName.ToLower();
            var subtitle = subtitleInformation.ToLower();

            var releaseGroup = DetermineReleaseGroup(release);
            var releaseResolution = DetermineResolution(release);
            var subtitleResolutions = DetermineSubtitleResolutions(subtitle);

            if (CheckIfReleaseAndSubtitleAreBothWebDl(release, subtitle))
                return true;

            if (CheckIfGroupShouldMatchRegardlesOfResolution(releaseGroup, subtitle))
                return true;

            if (CheckIfGroupsAreInterchangeable(releaseGroup, subtitle))
                return true;

            var foundReleaseGroupMatch = CheckIfReleaseGroupMatches(subtitle, releaseGroup);

            if (CheckIfSubtitleDoesntContainResolutionAndReleaseGroupsMatch(subtitleResolutions, foundReleaseGroupMatch))
                return true;

            if (CheckIfReleaseGroupAndResolutionMatch(subtitleResolutions, releaseResolution, foundReleaseGroupMatch))
                return true;

            return false;
        }

        private static List<string> DetermineSubtitleResolutions(string subtitle)
        {
            var subtitleResolutions = new List<string>();
            var subtitleResolutionRegex = new Regex(@"\d{3,4}(p|i)");
            foreach (Match resolutionMatch in subtitleResolutionRegex.Matches(subtitle))
            {
                subtitleResolutions.Add(resolutionMatch.Value);
            }
            return subtitleResolutions;
        }

        private static bool CheckIfReleaseGroupAndResolutionMatch(IEnumerable<string> subtitleResolutions,
            string releaseResolution,
            bool foundReleaseGroupMatch)
        {
            if (subtitleResolutions.Any(x => x.Equals(releaseResolution)) && foundReleaseGroupMatch)
                return true;

            return false;
        }

        private static bool CheckIfSubtitleDoesntContainResolutionAndReleaseGroupsMatch(
            IEnumerable<string> subtitleResolutions,
            bool foundReleaseGroupMatch)
        {
            return !subtitleResolutions.Any() && foundReleaseGroupMatch;
        }

        private static bool CheckIfReleaseGroupMatches(string lowerCaseSubtitleInformation, string releaseGroup)
        {
            return lowerCaseSubtitleInformation.Contains(releaseGroup);
        }

        private static bool CheckIfReleaseAndSubtitleAreBothWebDl(string lowerRelease, string lowerSubtitle)
        {
            return lowerRelease.Contains("web") && lowerSubtitle.Contains("web");
        }

        private static string DetermineReleaseGroup(string lowerRelease)
        {
            return Regex.Match(lowerRelease, @".*-(\w+)($|\..+)").Groups[1].Value;
        }

        private static bool CheckIfGroupShouldMatchRegardlesOfResolution(string releaseGroup, string lowerSubtitle)
        {
            var matchesRegardlessOfResolution = new[] {"2hd", "bia", "fov"};

            if (matchesRegardlessOfResolution.Any(x => x.Equals(releaseGroup)) && lowerSubtitle.Contains(releaseGroup))
                return true;
            return false;
        }

        private static bool CheckIfGroupsAreInterchangeable(string releaseGroup, string lowerSubtitle)
        {
            if (CheckForInterchangeableGroup(releaseGroup, lowerSubtitle, new[] {"dimension", "lol", "sys"}))
                return true;

            if (CheckForInterchangeableGroup(releaseGroup, lowerSubtitle, new[] {"xii", "asap", "immerse"}))
                return true;

            if (CheckForInterchangeableGroup(releaseGroup, lowerSubtitle, new[] {"remarkable", "excellence"}))
                return true;

            return false;
        }

        private static bool CheckForInterchangeableGroup(string releaseGroup, string lowerSubtitle,
            string[] interchangeableGroups)
        {
            return interchangeableGroups.Any(x => x.Equals(releaseGroup)) &&
                   interchangeableGroups.Any(lowerSubtitle.Contains);
        }

        private static string DetermineResolution(string lowerRelease)
        {
            return Regex.Match(lowerRelease, @"\d{3,4}(p|i)").Value;
        }
    }
}