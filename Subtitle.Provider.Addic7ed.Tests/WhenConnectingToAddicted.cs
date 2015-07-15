namespace Subtitle.Provider.Addic7ed.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WhenConnectingToAddicted
    {
        [Test]
        public void IfLoginResultContainsLogoutWeAreLoggedIn()
        {
            "logout.php".IsLoggedIn().Should().BeTrue();
        }

        [Test]
        public void IfLoginResultDoesNotContainLogoutWeAreNotLoggedIn()
        {
            "login.php".IsLoggedIn().Should().BeFalse();
        }

        [Test]
        public void PasswordsShouldBeUrlEncoded()
        {
            var postdata = AddictedWebClientHelpers.CreateLoginPostData("blaat", "*aB12&34");

            postdata.Should().Be("username=blaat&password=*aB12%2634&Submit=Log+in");
        }

        [Test]
        public void TheResponseShouldBeReadCompletely()
        {
            const int bufferSize = 3000;
            var buffer = new Byte[bufferSize];

            for (var i = 0; i < bufferSize; i++)
            {
                buffer[i] = (byte) 'a';
            }
            var stream = new MemoryStream(buffer);

            var result = stream.ReadResponse();

            result.Count().Should().Be(bufferSize);
            foreach (var character in result)
            {
                character.Should().Be('a');
            }
        }

        [Test]
        public void TheResponseShouldBeEmptyIfBufferIsEmpty()
        {
            const int bufferSize = 0;
            var buffer = new Byte[bufferSize];

            var stream = new MemoryStream(buffer);

            var result = stream.ReadResponse();

            result.Should().BeEmpty();
        }
    }
}