using System;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader
{
    public interface INotifier
    {
        void ForDownloadedSubtitle(EpisodePage episodePage, SubtitleVersion subtitleVersion,
            Provider.Addic7ed.Subtitle subtitle,
            SubtitleLink link, string downloadedTo, string linktToEpisode);

        void ForException(Exception exception, string message = "", EpisodePage episodePage = null,
            Provider.Addic7ed.Subtitle version = null, SubtitleLink link = null);

        void ForDownloadCountExceeded();
    }
}