namespace Subtitle.Provider.Addic7ed
{
    using System;

    public class FoundLink
    {
        public DateTime FoundOn { get; set; }
        public string Link { get; set; }
        public string ShowName { get; set; }
        public string SeasonEpisode { get; set; }
        public bool IgnoreAge { get; set; }
    }
}