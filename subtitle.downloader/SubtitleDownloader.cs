namespace Subtitle.Downloader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using System.Text;
    using Common;
    using Provider.Addic7ed;

    public class SubtitleDownloader
    {
        private readonly IDownload download;
        private readonly IFileSystem fileSystem;
        private readonly int maximumDaysOld;
        private readonly IMediaFinder mediaFinder;
        private readonly INotifier notifier;
        private readonly IEnumerable<string> orderedAllowedLanguages;
        private readonly List<DownloadedSub> previouslyDownloadedSubs;

        public SubtitleDownloader(IMediaFinder mediaFinder, IDownload download, IFileSystem fileSystem,
            List<DownloadedSub> previouslyDownloadedSubs, INotifier notifier, int maximumDaysOld,
            IEnumerable<string> orderedAllowedLanguages)
        {
            this.maximumDaysOld = maximumDaysOld;
            this.notifier = notifier;
            this.previouslyDownloadedSubs = previouslyDownloadedSubs;
            this.fileSystem = fileSystem;
            this.download = download;
            this.mediaFinder = mediaFinder;
            this.orderedAllowedLanguages = orderedAllowedLanguages;
        }

        public void For(List<FoundLink> foundLinks)
        {
            var handledLinks = new List<FoundLink>();
            foreach (var foundLink in foundLinks)
            {
                try
                {
                    var episodeInfo = foundLink.GetEpisodeInfo();
                    if (!mediaFinder.LookFor(episodeInfo.ShowName, episodeInfo.SeasonEpisode).Any())
                    {
                        handledLinks.Add(foundLink);
                        continue;
                    }

                    EpisodePage episodePage;
                    var pageContent = "";
                    try
                    {
                        pageContent = download.From(foundLink.Link);
                        episodePage = AddictedEpisodePageParser.For(pageContent);
                    }
                    catch (EpisodePageNoLongerExists)
                    {
                        handledLinks.Add(foundLink);
                        continue;
                    }
                    catch (Exception exception)
                    {
                        notifier.ForException(exception, string.Format("Error parsing link: {0}\n\nContent: {1}", foundLink.Link, pageContent));
                        handledLinks.Add(foundLink);
                        continue;
                    }

                    if (maximumDaysOld > 0
                        && episodePage.OldestAge.TotalHours >= maximumDaysOld*24
                        && !foundLink.IgnoreAge)
                    {
                        handledLinks.Add(foundLink);
                        continue;
                    }

                    var downloadExeeded = false;
                    foreach (var media in mediaFinder.LookFor(episodePage.ShowName, episodePage.SeasonEpisode))
                    {
                        var subtitleSearchResult = SubtitleSearch.For(media, episodePage.SubtitleVersions,
                            orderedAllowedLanguages);

                        if (subtitleSearchResult == null)
                            continue;

                        try
                        {
                            if (previouslyDownloadedSubs.Any(preDownloadedSub => 
                                preDownloadedSub.Link.Equals(subtitleSearchResult.Link.Link) 
                                && 
                                (string.IsNullOrEmpty(preDownloadedSub.For)
                                ||
                                preDownloadedSub.For.Equals(media.Path)
                                )))
                                continue;

                            var srtLocation = media.Path.Substring(0, media.Path.Length - 3) + subtitleSearchResult.Subtitle.Language.ToShortLanguage() + ".srt";
                            var srtContent = download.From(subtitleSearchResult.Link.Link, foundLink.Link);

                            if (srtContent.Contains("Daily Download count exceeded"))
                            {
                                downloadExeeded = true;
                                notifier.ForDownloadCountExceeded();
                                continue;
                            }

                            SaveStringToFile(fileSystem, srtLocation, srtContent);

                            previouslyDownloadedSubs.Add(new DownloadedSub
                            {
                                Link = subtitleSearchResult.Link.Link,
                                On = DateTime.Now,
                                For = media.Path
                            });

                            notifier.ForDownloadedSubtitle(episodePage, subtitleSearchResult.Version,
                                subtitleSearchResult.Subtitle, subtitleSearchResult.Link, srtLocation, foundLink.Link);
                        }
                        catch (Exception exception)
                        {
                            if (subtitleSearchResult != null)
                            {
                                notifier.ForException(exception, "Broken in download for", episodePage, subtitleSearchResult.Subtitle,
                                    subtitleSearchResult.Link);
                            }
                            else
                            {
                                notifier.ForException(exception);
                            }
                        }
                    }
                    if (downloadExeeded)
                        break;

                    handledLinks.Add(foundLink);
                }
                catch (Exception exception)
                {
                    notifier.ForException(exception);
                }
            }

            foundLinks.RemoveAll(l => handledLinks.Any(x => l == x));
        }

        public static void SaveStringToFile(IFileSystem fileSystem, string fileLocation, string content)
        {
            using (var sw = new StreamWriter(fileSystem.File.Open(fileLocation, FileMode.Create), Encoding.UTF8))
            {
                sw.Write(content);
            }
        }
    }
}