namespace Subtitle.Downloader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    public class MediaFinderBase
    {
        private readonly string[] allowedMediaExtensions = {".mkv", ".avi", ".mp4"};
        private readonly string mediaRootPath;
        protected IFileSystem FileSystem;

        public MediaFinderBase(string mediaRootPath, IFileSystem fileSystem = null)
        {
            FileSystem = fileSystem ?? new FileSystem();

            this.mediaRootPath = mediaRootPath;

            if (!FileSystem.Directory.Exists(mediaRootPath))
                throw new ArgumentException("Media path not found: " + mediaRootPath);
        }

        protected IEnumerable<string> GetFilesEpisodeDirectory(string showDirectory, string episode)
        {
            var episodeDirectory = Path.Combine(showDirectory, episode);

            if (!FileSystem.Directory.Exists(episodeDirectory))
                return new List<string>();

            return FileSystem.Directory.GetFiles(episodeDirectory);
        }

        protected bool CheckIfTheFileHasAMediaExtension(string fileToCheck)
        {
            return allowedMediaExtensions.Any(e => e.Equals(Path.GetExtension(fileToCheck)));
        }

        protected string GetDirectoryName(string fullPath)
        {
			var pathWithoutFinalSlash = fullPath.TrimEnd(Path.DirectorySeparatorChar);
			return pathWithoutFinalSlash.Substring(pathWithoutFinalSlash.LastIndexOf(Path.DirectorySeparatorChar) + 1);         
        }

        protected IEnumerable<string> GetAllShowDirectories()
        {
            return FileSystem.Directory.GetDirectories(mediaRootPath);
        }
    }
}