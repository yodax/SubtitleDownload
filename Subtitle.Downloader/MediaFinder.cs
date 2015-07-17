using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Subtitle.Downloader
{
    public class MediaFinder : MediaFinderBase, IMediaFinder
    {
        public MediaFinder(string mediaRootPath, IFileSystem fileSystem = null)
            : base(mediaRootPath, fileSystem)
        {
        }

        public IEnumerable<Media> LookFor(string showname, string episode)
        {
            var showDirectories = GetAllShowDirectories()
                .Where(d =>
                    AproximateCompare.With(showname, GetDirectoryName(d)));

            foreach (var showDirectory in showDirectories)
            {
                var allMediaFilesForTheEpisode =
                    GetFilesEpisodeDirectory(showDirectory, episode)
                        .Where(CheckIfTheFileHasAMediaExtension);

                foreach (var file in allMediaFilesForTheEpisode)
                {
                    yield return new Media
                    {
                        Name = Path.GetFileName(file),
                        Path = file
                    };
                }
            }
        }
    }
}