namespace Subtitle.Downloader
{
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using Provider.Addic7ed;

    public class ListPersist
    {
        private const string FoundLinksStoreName = "FoundLinks";
        private const string DownloadedSubsStoreName = "DownloadedSubs";
        private readonly IFileSystem fileSystem;

        public ListPersist(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void ToDisk(List<FoundLink> foundLinks)
        {
            // Save storage to disk
            foundLinks.Store(FoundLinksStoreName, fileSystem);
        }

        public void ToDisk(List<DownloadedSub> downloadedSubs)
        {
            // Save storage to disk
            downloadedSubs.Store(DownloadedSubsStoreName, fileSystem);
        }

        public List<DownloadedSub> FromDisk(List<DownloadedSub> downloadedSubs)
        {
            return downloadedSubs.Load(DownloadedSubsStoreName, fileSystem);
        }

        public List<FoundLink> FromDisk(List<FoundLink> foundLinks)
        {
            return foundLinks.Load(FoundLinksStoreName, fileSystem);
        }
    }
}