using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading;
using Subtitle.Common;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader
{
    public class SubtitleSession
    {
        private readonly IDownload _download;
        private readonly List<DownloadedSub> _downloadedSubs;
        private readonly List<string> _feedLinks;
        private readonly IFileSystem _fileSystem;
        private readonly List<FoundLink> _foundLinks;
        private readonly int _maximumDaysOld;
        private readonly IMediaFinder _mediaFinder;
        private readonly INotifier _notify;
        private readonly TimeSpan _oneMinute = new TimeSpan(0, 0, 1, 0);
        private readonly IEnumerable<string> _orderedAllowedLanguages;
        private readonly ListPersist _persist;
        private readonly TimeSpan _thirthyMinutes = new TimeSpan(0, 0, 30, 0);

        public SubtitleSession(IDownload download, IFileSystem fileSystem, INotifier notify, IMediaFinder mediaFinder,
            List<FoundLink> foundLinks, List<string> feedLinks, List<DownloadedSub> downloadedSubs, int maximumDaysOld,
            IEnumerable<string> orderedAllowedLanguages, ListPersist persist)
        {
            _persist = persist;
            _downloadedSubs = downloadedSubs;
            _feedLinks = feedLinks;
            _foundLinks = foundLinks;
            _mediaFinder = mediaFinder;
            _notify = notify;
            _fileSystem = fileSystem;
            _download = download;
            _maximumDaysOld = maximumDaysOld;
            _orderedAllowedLanguages = orderedAllowedLanguages;
        }

        public void StartNewSession(SessionAction sessionAction, string searchShowName = null, string episode = null)
        {
            var linkFinder = new LinkFinder(_foundLinks, _download);

            if (sessionAction == SessionAction.Check || sessionAction == SessionAction.Download)
            {
                CheckAction(linkFinder);
            }

            if (sessionAction == SessionAction.Search)
            {
                if (!string.IsNullOrEmpty(episode))
                {
                    linkFinder.LookForLinksForEpisode(searchShowName, episode);
                }
                else
                {
                    linkFinder.LookForLinksFromShow(searchShowName);
                }
            }

            var subtitileDownload = new SubtitleDownloader(_mediaFinder, _download, _fileSystem, _downloadedSubs,
                _notify, _maximumDaysOld, _orderedAllowedLanguages);

            if (sessionAction == SessionAction.Download)
            {
                subtitileDownload.For(_foundLinks);
            }

            if (sessionAction == SessionAction.Deamon)
            {
                var lastRunTimeFeed = DateTime.Now - _thirthyMinutes;
                var lastRunTimeDownload = DateTime.Now - _thirthyMinutes;

                while (true)
                {
                    if (TimeCheck.ForInterval(_thirthyMinutes, lastRunTimeFeed))
                    {
                        CheckAction(linkFinder);
                        _persist.ToDisk(_downloadedSubs);
                        _persist.ToDisk(_foundLinks);
                        lastRunTimeFeed = DateTime.Now;
                    }

                    if (TimeCheck.ForTime(TimeCheck.TodayAt(18, 0), lastRunTimeDownload))
                    {
                        subtitileDownload.For(_foundLinks);
                        _persist.ToDisk(_downloadedSubs);
                        _persist.ToDisk(_foundLinks);
                        lastRunTimeDownload = DateTime.Now;
                    }

                    Thread.Sleep(_oneMinute);
                }
            }
        }

        private void CheckAction(LinkFinder linkFinder)
        {
            linkFinder.LookForLinksFromFeeds(_feedLinks);
        }
    }
}