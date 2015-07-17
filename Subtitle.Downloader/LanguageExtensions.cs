using System;

namespace Subtitle.Downloader
{
    public static class LanguageExtensions
    {
        public static string ToShortLanguage(this string fullLanguage)
        {
            if (fullLanguage.Equals("dutch", StringComparison.InvariantCultureIgnoreCase))
                return "nl";
            if (fullLanguage.Equals("english", StringComparison.InvariantCultureIgnoreCase))
                return "en";

            return fullLanguage;
        }
    }
}