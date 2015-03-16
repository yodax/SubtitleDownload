namespace Subtitle.Downloader
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    public class MediaFinderWithoutSubtitles : MediaFinderBase, IMediaFinder
    {
        public MediaFinderWithoutSubtitles(string mediaRootPath, IFileSystem fileSystem = null)
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
                    if (!HasMAtchingSubtitle(file))
                    {
                        yield return new Media
                        {
                            Name = Path.GetFileName(file),
                            Path = file,
                        };
                    }
                }
            }
        }

        private bool HasMAtchingSubtitle(string mediaFile)
        {
            if (mediaFile == null)
                return false;

            var directoryName = Path.GetDirectoryName(mediaFile) ?? "";

            var subtitleFile = Path.Combine(directoryName,
                Path.GetFileNameWithoutExtension(mediaFile) + ".srt");

            return FileSystem.File.Exists(subtitleFile);
        }
    }
}