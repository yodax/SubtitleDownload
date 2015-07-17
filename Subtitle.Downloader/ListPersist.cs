using System.Collections.Generic;
using System.IO.Abstractions;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader
{
    public class ListPersist
    {
        private const string FoundLinksStoreName = "FoundLinks";
        private const string DownloadedSubsStoreName = "DownloadedSubs";
        private readonly IFileSystem _fileSystem;

        public ListPersist(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void ToDisk(List<FoundLink> foundLinks)
        {
            // Save storage to disk
            foundLinks.Store(FoundLinksStoreName, _fileSystem);
        }

        public void ToDisk(List<DownloadedSub> downloadedSubs)
        {
            // Save storage to disk
            downloadedSubs.Store(DownloadedSubsStoreName, _fileSystem);
        }

        public List<DownloadedSub> FromDisk(List<DownloadedSub> downloadedSubs)
        {
            return downloadedSubs.Load(DownloadedSubsStoreName, _fileSystem);
        }

        public List<FoundLink> FromDisk(List<FoundLink> foundLinks)
        {
            return foundLinks.Load(FoundLinksStoreName, _fileSystem);
        }
    }
}