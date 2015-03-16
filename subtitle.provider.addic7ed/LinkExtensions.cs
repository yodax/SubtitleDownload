namespace Subtitle.Provider.Addic7ed
{
    using System.Text.RegularExpressions;

    public static class LinkExtensions
    {
        public static string StripAddictedEpisode(this string url)
        {
            // http://www.addic7ed.com/serie/Ripper_Street/2/7/Our_Betrayal_(1)
            var regex = new Regex(@"(.*serie/.+/\d*/\d*/)");

            return regex.Match(url).Groups[1].Value;
        }
    }
}