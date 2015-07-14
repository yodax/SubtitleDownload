namespace Subtitle.Downloader
{
    using Provider.Addic7ed;

    public class SubtitleSearchResult
    {
        public Subtitle Subtitle { get; set; }
        public SubtitleLink Link { get; set; }
        public SubtitleVersion Version { get; set; }
    }
}