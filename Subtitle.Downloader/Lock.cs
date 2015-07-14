namespace Subtitle.Downloader
{
    using System.IO.Abstractions;

    public class Lock
    {
        private const string LockFileName = "Download.lock";
        private readonly IFileSystem fileSystem;

        public Lock(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool Aquire()
        {
            if (fileSystem.File.Exists(LockFileName))
                return false;

            fileSystem.File.Create(LockFileName).Close();

            return true;
        }

        public void Release()
        {
            if (fileSystem.File.Exists(LockFileName))
                fileSystem.File.Delete(LockFileName);
        }
    }
}