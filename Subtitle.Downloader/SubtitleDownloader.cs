using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Subtitle.Common;
using Subtitle.Provider.Addic7ed;

namespace Subtitle.Downloader
{
    public class SubtitleDownloader
    {
        private readonly IDownload _download;
        private readonly IFileSystem _fileSystem;
        private readonly int _maximumDaysOld;
        private readonly IMediaFinder _mediaFinder;
        private readonly INotifier _notifier;
        private readonly IEnumerable<string> _orderedAllowedLanguages;
        private readonly List<DownloadedSub> _previouslyDownloadedSubs;

        public SubtitleDownloader(IMediaFinder mediaFinder, IDownload download, IFileSystem fileSystem,
            List<DownloadedSub> previouslyDownloadedSubs, INotifier notifier, int maximumDaysOld,
            IEnumerable<string> orderedAllowedLanguages)
        {
            _maximumDaysOld = maximumDaysOld;
            _notifier = notifier;
            _previouslyDownloadedSubs = previouslyDownloadedSubs;
            _fileSystem = fileSystem;
            _download = download;
            _mediaFinder = mediaFinder;
            _orderedAllowedLanguages = orderedAllowedLanguages;
        }

        public void For(List<FoundLink> foundLinks)
        {
            var handledLinks = new List<FoundLink>();
            foreach (var foundLink in foundLinks)
            {
                try
                {
                    var episodeInfo = foundLink.GetEpisodeInfo();
                    if (!_mediaFinder.LookFor(episodeInfo.ShowName, episodeInfo.SeasonEpisode).Any())
                    {
                        handledLinks.Add(foundLink);
                        continue;
                    }

                    EpisodePage episodePage;
                    var pageContent = "";
                    try
                    {
                        pageContent = _download.From(foundLink.Link);
                        episodePage = AddictedEpisodePageParser.For(pageContent);
                    }
                    catch (EpisodePageNoLongerExists)
                    {
                        handledLinks.Add(foundLink);
                        continue;
                    }
                    catch (Exception exception)
                    {
                        _notifier.ForException(exception,
                            string.Format("Error parsing link: {0}\n\nContent: {1}", foundLink.Link, pageContent));
                        handledLinks.Add(foundLink);
                        continue;
                    }

                    if (_maximumDaysOld > 0
                        && episodePage.OldestAge.TotalHours >= _maximumDaysOld*24
                        && !foundLink.IgnoreAge)
                    {
                        handledLinks.Add(foundLink);
                        continue;
                    }

                    var downloadExeeded = false;
                    foreach (var media in _mediaFinder.LookFor(episodePage.ShowName, episodePage.SeasonEpisode))
                    {
                        var subtitleSearchResult = SubtitleSearch.For(media, episodePage.SubtitleVersions,
                            _orderedAllowedLanguages);

                        if (subtitleSearchResult == null)
                            continue;

                        try
                        {
                            if (_previouslyDownloadedSubs.Any(preDownloadedSub =>
                                preDownloadedSub.Link.Equals(subtitleSearchResult.Link.Link)
                                &&
                                (string.IsNullOrEmpty(preDownloadedSub.For)
                                 ||
                                 preDownloadedSub.For.Equals(media.Path)
                                    )))
                                continue;

                            var srtLocation = media.Path.Substring(0, media.Path.Length - 3) +
                                              subtitleSearchResult.Subtitle.Language.ToShortLanguage() + ".srt";
                            var srtContent = _download.From(subtitleSearchResult.Link.Link, foundLink.Link);

                            if (srtContent.Contains("Daily Download count exceeded"))
                            {
                                downloadExeeded = true;
                                _notifier.ForDownloadCountExceeded();
                                continue;
                            }

                            SaveStringToFile(_fileSystem, srtLocation, srtContent);

                            _previouslyDownloadedSubs.Add(new DownloadedSub
                            {
                                Link = subtitleSearchResult.Link.Link,
                                On = DateTime.Now,
                                For = media.Path
                            });

                            _notifier.ForDownloadedSubtitle(episodePage, subtitleSearchResult.Version,
                                subtitleSearchResult.Subtitle, subtitleSearchResult.Link, srtLocation, foundLink.Link);
                        }
                        catch (Exception exception)
                        {
                            if (subtitleSearchResult != null)
                            {
                                _notifier.ForException(exception, "Broken in download for", episodePage,
                                    subtitleSearchResult.Subtitle,
                                    subtitleSearchResult.Link);
                            }
                            else
                            {
                                _notifier.ForException(exception);
                            }
                        }
                    }
                    if (downloadExeeded)
                        break;

                    handledLinks.Add(foundLink);
                }
                catch (Exception exception)
                {
                    _notifier.ForException(exception);
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