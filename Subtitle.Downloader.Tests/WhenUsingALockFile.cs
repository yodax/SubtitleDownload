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

        private bool _aquiredLock;
        private MockFileSystem _fileSystem;
        private Lock _lockFile;

        private void GivenAFileSystemWithALockPresent()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {MockUnixSupport.Path(@"c:\downloadTool\Download.lock"), new MockFileData("")}
            }, MockUnixSupport.Path(@"c:\downloadTool\"));
        }

        private void ALockShouldNotBeSet()
        {
            _aquiredLock.Should().BeFalse();
        }

        private void ALockFileShouldBePresentOnTheFileSystem()
        {
            _fileSystem.File.Exists("Download.lock").Should().BeTrue();
        }

        private void ALockShouldBeSet()
        {
            _aquiredLock.Should().BeTrue();
        }

        private void WhenIAquireALock()
        {
            _aquiredLock = _lockFile.Aquire();
        }

        private void WhenIReleaseALock()
        {
            _lockFile.Release();
        }

        private void GivenALockingMechanism()
        {
            _lockFile = new Lock(_fileSystem);
        }

        private void GivenAnEmptyFileSystem()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>(),
                MockUnixSupport.Path(@"c:\downloadTool\"));
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