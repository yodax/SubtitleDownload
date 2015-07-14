namespace Subtitle.Downloader
{
    using System.Collections.Generic;
    using System.Linq;
    using Provider.Addic7ed;

    public class SubtitleSearch
    {
        public static SubtitleSearchResult For(Media media, IEnumerable<SubtitleVersion> subtitleVersions, IEnumerable<string> orderedAllowedLanguages)
        {
            var allowedLanguages = orderedAllowedLanguages;

            var allSubtitlesSuitableForMedia = subtitleVersions
                .Where(s => CheckMatch.For(media.Name, s.Release));

            var results = (
                from subtitleVersion in allSubtitlesSuitableForMedia
                from subtitle in subtitleVersion.Subtitles
                    .Where(s => 
                        allowedLanguages.Any(a => s.Language.Equals(a))
                        &&
                        s.Completed
                    )
                from link in subtitle.Links
                select new {
                    LanguageId = allowedLanguages.ToList().IndexOf(subtitle.Language),
                    Result = new SubtitleSearchResult
                    {
                        Link = link,
                        Subtitle = subtitle,
                        Version = subtitleVersion
                    }
                }
                ).ToList();

            if (!results.Any()) return null;

            var prioList = results
                .OrderBy(x => x.LanguageId)
                .ThenBy(x => x.Result.Subtitle.HearingImpaired)
                .ThenByDescending(x => x.Result.Subtitle.Downloads)
                .ThenByDescending(x => x.Result.Link.Type);

            return prioList.First().Result;
        }
    }
}