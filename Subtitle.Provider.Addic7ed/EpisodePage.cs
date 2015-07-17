using System;
using System.Collections.Generic;
using System.Linq;

namespace Subtitle.Provider.Addic7ed
{
    public class EpisodePage
    {
        public EpisodePage(string showName, int episode, int season, string episodeName,
            IEnumerable<SubtitleVersion> subtitleVersions)
        {
            ShowName = showName;
            Episode = episode;
            Season = season;
            EpisodeName = episodeName;
            SeasonEpisode = ExtensionMethods.GenerateSeasonEpisode(season, episode);
            SubtitleVersions = subtitleVersions;
        }

        public string ShowName { get; private set; }
        public int Episode { get; private set; }
        public int Season { get; private set; }
        public string EpisodeName { get; private set; }
        public IEnumerable<SubtitleVersion> SubtitleVersions { get; private set; }
        public string SeasonEpisode { get; private set; }

        public TimeSpan OldestAge
        {
            get { return SubtitleVersions.Max(x => x.Age); }
        }
    }
}