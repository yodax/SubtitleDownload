namespace Subtitle.Downloader
{
    using System;
    using Provider.Addic7ed;

    public interface INotifier
    {
        void ForDownloadedSubtitle(EpisodePage episodePage, SubtitleVersion subtitleVersion, Subtitle subtitle,
            SubtitleLink link, string downloadedTo, string linktToEpisode);

        void ForException(Exception exception, string message = "", EpisodePage episodePage = null, Subtitle version = null, SubtitleLink link = null);

        void ForDownloadCountExceeded();
    }
}