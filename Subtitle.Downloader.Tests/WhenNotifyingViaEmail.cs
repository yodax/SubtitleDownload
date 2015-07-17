using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader.Tests
{
    [TestFixture]
    public class WhenNotifyingViaEmail
    {
        [SetUp]
        public void Setup()
        {
            _mailMock = Substitute.For<IMailer>();
            _notifier = new EmailNotifier(_mailMock);
        }

        private IMailer _mailMock;
        private EmailNotifier _notifier;

        private void MailShouldHaveBeenSendWith(string subject, string body)
        {
            _mailMock.Received().Send(Arg.Is(subject), Arg.Is(body));
        }

        [Test]
        public void IfASubtitleWasDownloadedAgeShouldBeInHours()
        {
            var link = new SubtitleLink
            {
                Link = "link",
                Type = SubtitleLinkType.Download
            };
            var subtitle = new Provider.Addic7ed.Subtitle
            {
                Downloads = 0,
                HearingImpaired = false,
                Language = "Dutch",
                Links = new List<SubtitleLink>
                {
                    link
                }
            };
            var version = new SubtitleVersion
            {
                Release = "KILLERS",
                Age = new TimeSpan(0, 7, 0, 0),
                Uploader = "elderman",
                Subtitles = new List<Provider.Addic7ed.Subtitle>
                {
                    subtitle
                }
            };

            var subtitleVersions = new List<SubtitleVersion>
            {
                version
            };
            var page = new EpisodePage("ShowName", 1, 1, "Episode name", subtitleVersions);

            _notifier.ForDownloadedSubtitle(page, version, subtitle, link, @"/tank/video/TV/blaat.srt", "linkToEpisode");
            var body = "Dutch subtitle was downloaded for ShowName S01E01" + Environment.NewLine
                       + Environment.NewLine
                       + @"Version age: 7 hours" + Environment.NewLine + Environment.NewLine
                       + @"Oldest age: 7 hours" + Environment.NewLine + Environment.NewLine
                       + @"To: /tank/video/TV/blaat.srt" + Environment.NewLine + Environment.NewLine
                       + @"Uploader: elderman" + Environment.NewLine + Environment.NewLine
                       + @"Episode: linkToEpisode" + Environment.NewLine + Environment.NewLine
                       + @"Version: KILLERS";

            MailShouldHaveBeenSendWith("Dutch Sub: ShowName S01E01", body);
        }

        [Test]
        public void IfASubtitleWasDownloadedWithAge1Day()
        {
            var link = new SubtitleLink
            {
                Link = "link",
                Type = SubtitleLinkType.Download
            };
            var subtitle = new Provider.Addic7ed.Subtitle
            {
                Downloads = 0,
                HearingImpaired = false,
                Language = "Dutch",
                Links = new List<SubtitleLink>
                {
                    link
                }
            };
            var version = new SubtitleVersion
            {
                Release = "KILLERS",
                Age = new TimeSpan(1, 0, 0, 0),
                Uploader = "elderman",
                Subtitles = new List<Provider.Addic7ed.Subtitle>
                {
                    subtitle
                }
            };

            var subtitleVersions = new List<SubtitleVersion>
            {
                version
            };
            var page = new EpisodePage("ShowName", 1, 1, "Episode name", subtitleVersions);

            _notifier.ForDownloadedSubtitle(page, version, subtitle, link, @"/tank/video/TV/blaat.srt", "linkToEpisode");
            var body = "Dutch subtitle was downloaded for ShowName S01E01" + Environment.NewLine
                       + Environment.NewLine
                       + @"Version age: 1 day" + Environment.NewLine + Environment.NewLine
                       + @"Oldest age: 1 day" + Environment.NewLine + Environment.NewLine
                       + @"To: /tank/video/TV/blaat.srt" + Environment.NewLine + Environment.NewLine
                       + @"Uploader: elderman" + Environment.NewLine + Environment.NewLine
                       + @"Episode: linkToEpisode" + Environment.NewLine + Environment.NewLine
                       + @"Version: KILLERS";
            MailShouldHaveBeenSendWith("Dutch Sub: ShowName S01E01", body);
        }

        [Test]
        public void IfASubtitleWasDownloadedWithAge2Days()
        {
            var link = new SubtitleLink
            {
                Link = "link",
                Type = SubtitleLinkType.Download
            };
            var subtitle = new Provider.Addic7ed.Subtitle
            {
                Downloads = 0,
                HearingImpaired = false,
                Language = "Dutch",
                Links = new List<SubtitleLink>
                {
                    link
                }
            };
            var version = new SubtitleVersion
            {
                Release = "KILLERS",
                Age = new TimeSpan(2, 0, 0, 0),
                Uploader = "elderman",
                Subtitles = new List<Provider.Addic7ed.Subtitle>
                {
                    subtitle
                }
            };

            var subtitleVersions = new List<SubtitleVersion>
            {
                version
            };
            var page = new EpisodePage("ShowName", 1, 1, "Episode name", subtitleVersions);

            _notifier.ForDownloadedSubtitle(page, version, subtitle, link, @"/tank/video/TV/blaat.srt", "LinkToEpisode");
            var body = "Dutch subtitle was downloaded for ShowName S01E01" + Environment.NewLine
                       + Environment.NewLine
                       + @"Version age: 2 days" + Environment.NewLine + Environment.NewLine
                       + @"Oldest age: 2 days" + Environment.NewLine + Environment.NewLine
                       + @"To: /tank/video/TV/blaat.srt" + Environment.NewLine + Environment.NewLine
                       + @"Uploader: elderman" + Environment.NewLine + Environment.NewLine
                       + @"Episode: LinkToEpisode" + Environment.NewLine + Environment.NewLine
                       + @"Version: KILLERS";
            MailShouldHaveBeenSendWith("Dutch Sub: ShowName S01E01", body);
        }

        [Test]
        public void IfDownloadCountWasExceeded()
        {
            _notifier.ForDownloadCountExceeded();

            MailShouldHaveBeenSendWith("Download count exceeded for addic7ed",
                "Download count exceeded for addic7ed. Please get a VIP subscription.");
        }
    }
}