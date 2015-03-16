namespace Subtitle.Downloader
{
    using System;
    using System.Text;
    using Provider.Addic7ed;

    public class EmailNotifier : INotifier
    {
        private readonly IMailer email;

        public EmailNotifier(string smtpServer, string mailFrom, string mailTo)
        {
            email = new Mailer(smtpServer, mailFrom, mailTo);
        }

        public EmailNotifier(IMailer mailer)
        {
            email = mailer;
        }

        public void ForDownloadedSubtitle(EpisodePage episodePage, SubtitleVersion subtitleVersion, Subtitle subtitle,
            SubtitleLink link, string downloadedTo, string linkToEpisode)
        {
            var subject = string.Format("{0} Sub: {1} {2}", subtitle.Language, episodePage.ShowName,
                episodePage.SeasonEpisode);
            var body = string.Format(
                "{0} subtitle was downloaded for {1} {2}" + Environment.NewLine + Environment.NewLine
                + "Version age: {3}" + Environment.NewLine + Environment.NewLine
                + "Oldest age: {4}" + Environment.NewLine + Environment.NewLine
                + "To: {5}" + Environment.NewLine + Environment.NewLine
                + "Uploader: {6}" + Environment.NewLine + Environment.NewLine
                + "Episode: {7}" + Environment.NewLine + Environment.NewLine
                + "Version: {8}"
                , subtitle.Language,
                episodePage.ShowName,
                episodePage.SeasonEpisode,
                AgeToString(subtitleVersion.Age),
                AgeToString(episodePage.OldestAge),
                downloadedTo,
                subtitleVersion.Uploader,
                linkToEpisode,
                subtitleVersion.Release);

            email.Send(subject, body);
        }

        public void ForException(Exception exception, string message = null, EpisodePage episodePage = null, Subtitle version = null, SubtitleLink link = null)
        {
            const string subject = "Error when downloading subs";

            var body = new StringBuilder();

            if (episodePage != null)
                body.AppendLine(string.Format("Page: {0} {1}", episodePage.ShowName, episodePage.SeasonEpisode));

            body.AppendLine();

            if (version != null && link != null)
                body.AppendLine(string.Format("Subtitle version: {0} {1}", version.Language, link.Type));

            body.AppendLine();

            body.AppendLine(string.Format("Message: {0}", message));

            body.AppendLine();

            body.AppendLine(string.Format("Exception {0}: {1}", exception.GetType(), exception.Message));

            body.AppendLine();

            body.AppendLine(string.Format("Trace: {0}", exception.StackTrace));

            email.Send(subject, body.ToString());
        }

        public void ForDownloadCountExceeded()
        {
            const string subject = "Download count exceeded for addic7ed";
            const string body = "Download count exceeded for addic7ed. Please get a VIP subscription.";

            email.Send(subject, body);
        }

        private string AgeToString(TimeSpan age)
        {
            if (age.TotalDays > 1)
                return string.Format("{0} days", age.TotalDays);

            if (age.TotalDays == 1)
                return string.Format("{0} day", age.TotalDays);

            return string.Format("{0} hours", age.TotalHours);
        }
    }
}