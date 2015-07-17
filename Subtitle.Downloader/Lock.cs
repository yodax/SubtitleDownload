using System.IO.Abstractions;

namespace Subtitle.Downloader
{
    public class Lock
    {
        private const string LockFileName = "Download.lock";
        private readonly IFileSystem _fileSystem;

        public Lock(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool Aquire()
        {
            if (_fileSystem.File.Exists(LockFileName))
                return false;

            _fileSystem.File.Create(LockFileName).Close();

            return true;
        }

        public void Release()
        {
            if (_fileSystem.File.Exists(LockFileName))
                _fileSystem.File.Delete(LockFileName);
        }
    }
}