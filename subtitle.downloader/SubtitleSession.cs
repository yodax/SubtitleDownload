namespace Subtitle.Downloader
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Threading;
    using Common;
    using Provider.Addic7ed;

    public class SubtitleSession
    {
        private readonly IDownload download;
        private readonly List<DownloadedSub> downloadedSubs;
        private readonly List<string> feedLinks;
        private readonly IFileSystem fileSystem;
        private readonly List<FoundLink> foundLinks;
        private readonly int maximumDaysOld;
        private readonly IMediaFinder mediaFinder;
        private readonly INotifier notify;
        private readonly IEnumerable<string> orderedAllowedLanguages;
        private readonly ListPersist persist;
        private readonly TimeSpan oneMinute = new TimeSpan(0, 0, 1, 0);
        private readonly TimeSpan thirthyMinutes = new TimeSpan(0, 0, 30, 0);

        public SubtitleSession(IDownload download, IFileSystem fileSystem, INotifier notify, IMediaFinder mediaFinder, List<FoundLink> foundLinks, List<string> feedLinks, List<DownloadedSub> downloadedSubs, int maximumDaysOld, IEnumerable<string> orderedAllowedLanguages, ListPersist persist)
        {
            this.persist = persist;
            this.downloadedSubs = downloadedSubs;
            this.feedLinks = feedLinks;
            this.foundLinks = foundLinks;
            this.mediaFinder = mediaFinder;
            this.notify = notify;
            this.fileSystem = fileSystem;
            this.download = download;
            this.maximumDaysOld = maximumDaysOld;
            this.orderedAllowedLanguages = orderedAllowedLanguages;
        }

        public void StartNewSession(SessionAction sessionAction, string searchShowName = null, string episode = null)
        {
            var linkFinder = new LinkFinder(foundLinks, download);

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

            var subtitileDownload = new SubtitleDownloader(mediaFinder, download, fileSystem, downloadedSubs,
                                    notify, maximumDaysOld, orderedAllowedLanguages);

            if (sessionAction == SessionAction.Download)
            {
                subtitileDownload.For(foundLinks);
            }

            if (sessionAction == SessionAction.Deamon)
            {
                var lastRunTimeFeed = DateTime.Now - thirthyMinutes;
                var lastRunTimeDownload = DateTime.Now - thirthyMinutes;

                while (true)
                {
                    if (TimeCheck.ForInterval(thirthyMinutes, lastRunTimeFeed))
                    {
                        CheckAction(linkFinder);
                        persist.ToDisk(downloadedSubs);
                        persist.ToDisk(foundLinks);
                        lastRunTimeFeed = DateTime.Now;
                    }

                    if (TimeCheck.ForTime(TimeCheck.TodayAt(18, 0), lastRunTimeDownload))
                    {
                        subtitileDownload.For(foundLinks);
                        persist.ToDisk(downloadedSubs);
                        persist.ToDisk(foundLinks);
                        lastRunTimeDownload = DateTime.Now;
                    }

                    Thread.Sleep(oneMinute);
                }
            }
        }

        private void CheckAction(LinkFinder linkFinder)
        {
            linkFinder.LookForLinksFromFeeds(feedLinks);
        }
    }
}