using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Downloader.Tests
{
    [TestFixture]
    public class WhenUsingALockFile
    {
        [SetUp]
        public void Initialize()
        {
            GivenAnEmptyFileSystem();

            GivenALockingMechanism();
        }

        private bool aquiredLock;
        private MockFileSystem fileSystem;
        private Lock lockFile;

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

        [Test]
        public void IfALockFileExistsNoLockShouldBeAquired()
        {
            GivenAFileSystemWithALockPresent();

            GivenALockingMechanism();

            WhenIAquireALock();

            ALockShouldNotBeSet();
        }

        [Test]
        public void IfALockIsReleasedALockShouldBeAvailable()
        {
            WhenIAquireALock();

            WhenIReleaseALock();

            WhenIAquireALock();

            ALockFileShouldBePresentOnTheFileSystem();
        }

        [Test]
        public void IfNoLockFileExistsAFileShouldBeCreated()
        {
            WhenIAquireALock();

            ALockFileShouldBePresentOnTheFileSystem();
        }

        [Test]
        public void IfNoLockFileExistsALockShouldBeAquired()
        {
            WhenIAquireALock();

            ALockShouldBeSet();
        }

        [Test]
        public void TwoConsecutiveAquiresShouldFail()
        {
            WhenIAquireALock();

            WhenIAquireALock();

            ALockShouldNotBeSet();
        }
    }
}