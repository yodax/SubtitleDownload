using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader
{
    public class SubtitleSearchResult
    {
        public Provider.Addic7ed.Subtitle Subtitle { get; set; }
        public SubtitleLink Link { get; set; }
        public SubtitleVersion Version { get; set; }
    }
}