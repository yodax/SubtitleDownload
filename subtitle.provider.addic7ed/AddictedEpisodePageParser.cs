namespace Subtitle.Provider.Addic7ed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using ScrapySharp.Extensions;

    public static class AddictedEpisodePageParser
    {
        public static EpisodePage For(string pageContent)
        {
            if (String.IsNullOrEmpty(pageContent))
                throw new ArgumentException(@"Page is empty", "pageContent");

            if (pageContent.Contains("<td valign=\"top\" class=\"newsDate\">"))
                throw new EpisodePageNoLongerExists("Episode page no longer exists");

            var html = new HtmlDocument();
            html.LoadHtml(pageContent);

            var titleNode = html.DocumentNode.FindAddictedTitleNode();
            if (titleNode == null)
                throw new ParsingException("No title was found on the page");

            var showInformation = ExtractShowInformation(titleNode);

            var subtitleVersions = ExtractSubtitleVersions(html.DocumentNode);

            return new EpisodePage(
                showInformation.ShowName,
                showInformation.Episode,
                showInformation.Season,
                showInformation.EpisodeName,
                subtitleVersions);
        }

        private static AddictedShowInformation ExtractShowInformation(HtmlNode titleNode)
        {
            var positionOfHtmlTag = titleNode.InnerHtml.IndexOf('<');
            var titleText = titleNode.InnerHtml.Substring(0, positionOfHtmlTag).Trim();
            var titleParts = titleText.Split(new[] {" - "}, StringSplitOptions.RemoveEmptyEntries);
            var episodeString = titleParts[1];

            return new AddictedShowInformation
            {
                Episode = Convert.ToInt32(episodeString.Split('x')[1]),
                Season = Convert.ToInt32(episodeString.Split('x')[0]),
                EpisodeName = titleParts[2].Trim(),
                ShowName = titleParts[0].Trim()
            };
        }

        private static IEnumerable<SubtitleVersion> ExtractSubtitleVersions(HtmlNode documentNode)
        {
            var subtitleVersions = new List<SubtitleVersion>();

            foreach (var subtitleNode in documentNode.FindSubtitleNodes())
            {
                var subtitleVersion = new SubtitleVersion
                {
                    Release = subtitleNode.FindSubtitleRelease(),
                    Age = subtitleNode.FindTheAge(),
                    Uploader = subtitleNode.FindUploader()
                };

                var languageNodes = subtitleNode.FindLanguageNodes();

                if (languageNodes == null)
                    continue;

                foreach (var languageNode in languageNodes)
                {
                    subtitleVersion.Subtitles.Add(new Subtitle
                    {
                        Language = languageNode.FindLanguage(),
                        HearingImpaired = languageNode.NextSibling.NextSibling.IsHearingImpaired(),
                        Downloads = languageNode.NextSibling.NextSibling.FindDownloadCount(),
                        Links = languageNode.FindTheLinks(),
                        Completed = languageNode.CheckIfTheSubtitleIsCompleted()
                    });
                }
                subtitleVersions.Add(subtitleVersion);
            }

            return subtitleVersions;
        }

        private static HtmlNode FindAddictedTitleNode(this HtmlNode node)
        {
            return node.CssSelect("span.titulo").FirstOrDefault();
        }

        private static IEnumerable<HtmlNode> FindSubtitleNodes(this HtmlNode node)
        {
            return node.SelectNodes("//div[@id='container95m']/table[@class='tabel95']//table[@class='tabel95']");
        }

        private static string FindSubtitleRelease(this HtmlNode node)
        {
            var titleNode = node.SelectNodes(".//td[@class='NewsTitle']").First();

            var releaseText = titleNode.InnerText;
            var positionOfComma = releaseText.IndexOf(',');
            var lengthOfVersion = "Version ".Length;
            var mainInfo = releaseText.Substring(lengthOfVersion, positionOfComma - lengthOfVersion);

            var extraInfo = "";
            var extraInfoNode = node.SelectNodes(".//td[@class='newsDate']").FirstOrDefault();
            if (extraInfoNode != null && !String.IsNullOrEmpty(extraInfoNode.InnerText.Trim()))
                extraInfo = " " + extraInfoNode.InnerText.Trim();

            return mainInfo + extraInfo;
        }

        private static string FindUploader(this HtmlNode node)
        {
            var uploaderLink = node.SelectSingleNode(".//a[contains(@href, '/user/')]");
            
            if (uploaderLink == null)
                return "";

            return uploaderLink.InnerText;
        }

        private static TimeSpan FindTheAge(this HtmlNode node)
        {
            var regex = new Regex(@"(\d+) (days|hours) ago");
            var match = regex.Match(node.InnerText);

            if (match.Groups[0].Value.Equals(""))
                return new TimeSpan();

            var number = Convert.ToInt32(match.Groups[1].Value);
            var quantifier = match.Groups[2].Value;

            if (quantifier.Equals("hours"))
            {
                return new TimeSpan(0, number, 0, 0);
            }

            return new TimeSpan(number, 0, 0, 0);
        }

        private static IEnumerable<HtmlNode> FindLanguageNodes(this HtmlNode node)
        {
            try
            {
                return node.SelectNodes(".//td[@class='language']").Select(languageNode => languageNode.ParentNode);
            }
            catch
            {
                return null;
            }
        }

        private static bool CheckIfTheSubtitleIsCompleted(this HtmlNode node)
        {
            var statusNode = node.SelectSingleNode(".//td//b");

            try
            {
                if (statusNode.InnerText.Trim().Equals("Completed", StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            catch {}
            
            return false;
        }
        
        private static string FindLanguage(this HtmlNode node)
        {
            return node.SelectSingleNode(".//td[@class='language']").InnerText.Trim();
        }

        private static bool IsHearingImpaired(this HtmlNode node)
        {
            return node.SelectSingleNode(".//img[@title='Hearing Impaired']") != null;
        }

        private static int FindDownloadCount(this HtmlNode node)
        {
            var regex = new Regex(@"(\d*) Downloads");

            var matches = regex.Match(node.InnerText);

            if (matches.Groups.Count > 1)
                return Convert.ToInt32(matches.Groups[1].Value);

            return 0;
        }

        private static List<SubtitleLink> FindTheLinks(this HtmlNode node)
        {
            var links = new List<SubtitleLink>();

            var anchorTags = node.SelectNodes(".//a[@class='buttonDownload']") ?? new HtmlNodeCollection(node);
            var otherAnchorTags = node.SelectNodes(".//a[text()='Download']") ?? new HtmlNodeCollection(node);

            foreach (var anchorTag in anchorTags.Union(otherAnchorTags))
            {
                var addictedLink = new SubtitleLink();
                var buttonText = anchorTag.InnerText.ToLower();
                if (buttonText.Equals("original") || buttonText.Equals("download"))
                {
                    addictedLink.Type = SubtitleLinkType.Download;
                }
                else
                {
                    addictedLink.Type = SubtitleLinkType.Updated;
                }
                addictedLink.Link = @"http://www.addic7ed.com" + anchorTag.Attributes["href"].Value;
                links.Add(addictedLink);
            }
            return links;
        }

        private class AddictedShowInformation
        {
            public int Season { get; set; }
            public int Episode { get; set; }
            public string EpisodeName { get; set; }
            public string ShowName { get; set; }
        }
    }

    public class EpisodePageNoLongerExists : Exception
    {
        public EpisodePageNoLongerExists(string message): base(message)
        {
            
        }
    }
}