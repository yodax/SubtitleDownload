using System;
using System.Collections.Generic;

namespace Subtitle.Downloader
{
    public static class DownloadedSubExtensions
    {
        public static List<DownloadedSub> RemoveOlderThan(this List<DownloadedSub> downloadedSubs, int days)
        {
            downloadedSubs.RemoveAll(sub => sub.On <= DateTime.Now.AddDays(days*-1));

            return downloadedSubs;
        }
    }
}