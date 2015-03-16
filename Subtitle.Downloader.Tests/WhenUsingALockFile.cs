namespace Subtitle.Downloader.Tests
{
    using System.Collections.Generic;
    using System.IO.Abstractions.TestingHelpers;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenUsingALockFile
    {
        private bool aquiredLock;
        private MockFileSystem fileSystem;
        private Lock lockFile;

        [TestInitialize]
        public void Initialize()
        {
            GivenAnEmptyFileSystem();

            GivenALockingMechanism();
        }

        [TestMethod]
        public void IfNoLockFileExistsALockShouldBeAquired()
        {
            WhenIAquireALock();

            ALockShouldBeSet();
        }

        [TestMethod]
        public void IfNoLockFileExistsAFileShouldBeCreated()
        {
            WhenIAquireALock();

            ALockFileShouldBePresentOnTheFileSystem();
        }

        [TestMethod]
        public void IfALockFileExistsNoLockShouldBeAquired()
        {
            GivenAFileSystemWithALockPresent();

            GivenALockingMechanism();

            WhenIAquireALock();

            ALockShouldNotBeSet();
        }

        [TestMethod]
        public void TwoConsecutiveAquiresShouldFail()
        {
            WhenIAquireALock();

            WhenIAquireALock();

            ALockShouldNotBeSet();
        }

        [TestMethod]
        public void IfALockIsReleasedALockShouldBeAvailable()
        {
            WhenIAquireALock();

            WhenIReleaseALock();

            WhenIAquireALock();

            ALockFileShouldBePresentOnTheFileSystem();
        }

        private void GivenAFileSystemWithALockPresent()
        {
            fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\downloadTool\Download.lock", new MockFileData("")}
            }, @"c:\downloadTool\");
        }

        private void ALockShouldNotBeSet()
        {
            aquiredLock.Should().BeFalse();
        }

        private void ALockFileShouldBePresentOnTheFileSystem()
        {
            fileSystem.File.Exists("Download.lock").Should().BeTrue();
        }

        private void ALockShouldBeSet()
        {
            aquiredLock.Should().BeTrue();
        }

        private void WhenIAquireALock()
        {
            aquiredLock = lockFile.Aquire();
        }

        private void WhenIReleaseALock()
        {
            lockFile.Release();
        }

        private void GivenALockingMechanism()
        {
            lockFile = new Lock(fileSystem);
        }

        private void GivenAnEmptyFileSystem()
        {
            fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>(), @"c:\downloadTool\");
        }
    }
}