using System.Collections.Generic;

namespace Subtitle.Provider.Addic7ed
{
    public class Subtitle
    {
        public Subtitle()
        {
            Links = new List<SubtitleLink>();
        }

        public string Language { get; set; }
        public List<SubtitleLink> Links { get; set; }
        public bool HearingImpaired { get; set; }
        public int Downloads { get; set; }
        public bool Completed { get; set; }
    }
}