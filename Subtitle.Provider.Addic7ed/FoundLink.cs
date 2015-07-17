using System;

namespace Subtitle.Provider.Addic7ed
{
    public class FoundLink
    {
        public DateTime FoundOn { get; set; }
        public string Link { get; set; }
        public string ShowName { get; set; }
        public string SeasonEpisode { get; set; }
        public bool IgnoreAge { get; set; }
    }
}