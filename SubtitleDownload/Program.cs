namespace SubtitleDownload
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;
    using Properties;
    using Subtitle.Downloader;
    using Subtitle.Provider.Addic7ed;

    internal class Program
    {
        private const string RssLinksStoreName = "FeedLinks";

        private static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine(@"Please supply a commandline argument of either Check or Download");
                return;
            }

            try
            {
                var commandArguments = new List<string>(args);
                var switches = commandArguments.Where(a => a.StartsWith("-")).ToList();
                switches.ForEach(x => commandArguments.Remove(x));

                // Initialize non test versions of classes
                var notify = new EmailNotifier(Settings.Default.SmtpServer, Settings.Default.MailFrom,
                    Settings.Default.MailTo);
                var download = new Download(Settings.Default.AddictedUsername, Settings.Default.AddictedPassword);
                var fileSystem = new FileSystem();

                // Aquire a lock on all files
                var lockFile = new Lock(fileSystem);
                if (!lockFile.Aquire())
                    return;
                
                try
                {
                    // Check for switches
                    IMediaFinder mediaFinder = new MediaFinder(Settings.Default.VideoPath, fileSystem);
                    if (switches.Any(s => s.Equals("-nosrt")))
                        mediaFinder = new MediaFinderWithoutSubtitles(Settings.Default.VideoPath, fileSystem);

                    // Load storage from disk
                    var persist = new ListPersist(fileSystem);
                    var foundLinks = persist.FromDisk(new List<FoundLink>());
                    var downloadedSubs = persist.FromDisk(new List<DownloadedSub>());
                    var rssLinks = new List<string>().Load(RssLinksStoreName, fileSystem);

                    // Initialize rssfeeds if not found
                    if (!rssLinks.Any())
                    {
                        rssLinks.Add("http://www.addic7ed.com/rss.php?mode=hotspot");
                        rssLinks.Add("http://www.addic7ed.com/rss.php?mode=translated");
                        rssLinks.Add("http://feeds2.feedburner.com/Addic7edLastNewVersions");
                        rssLinks.Add("http://feeds2.feedburner.com/Addic7edLastUploaded");
                        rssLinks.Store(RssLinksStoreName, fileSystem);
                    }

                    var orderedAllowedLanguages = Settings.Default.OrderedAllowedLanguages.Split(',');
                    var maximumDaysOld = Settings.Default.MaximumAgeOfFirstUpload;

                    // Start new session
                    var suppliedCommand = commandArguments[0].ToLower();
                    var searchTerm = commandArguments.Count >= 2 ? commandArguments[1] : "";
                    var episodeTerm = commandArguments.Count == 3 ? commandArguments[2] : "";

                    var sessionAction = SessionAction.Check;

                    switch (suppliedCommand)
                    {
                        case "check":
                        {
                            sessionAction = SessionAction.Check;
                            break;
                        }
                        case "download":
                        {
                            sessionAction = SessionAction.Download;
                            break;
                        }
                        case "search":
                        {
                            sessionAction = SessionAction.Search;
                            break;
                        }
                        case "deamon":
                        {
                            sessionAction = SessionAction.Deamon;
                            break;
                        }
                    }

                    var subtitleSession = new SubtitleSession(
                        download,
                        fileSystem,
                        notify,
                        mediaFinder,
                        foundLinks,
                        rssLinks,
                        downloadedSubs,
                        maximumDaysOld,
                        orderedAllowedLanguages,
                        persist);

                    subtitleSession.StartNewSession(sessionAction, searchTerm, episodeTerm);

                    persist.ToDisk(foundLinks);
                    persist.ToDisk(downloadedSubs.RemoveOlderThan(maximumDaysOld));
                }
                finally
                {
                    lockFile.Release();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                // Always release the lock
                new Lock(new FileSystem()).Release();
            }
            
        }
    }
}