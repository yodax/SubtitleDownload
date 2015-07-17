using System.Globalization;

namespace Subtitle.Provider.Addic7ed
{
    public static class ExtensionMethods
    {
        public static string GenerateSeasonEpisode(int season, int episode)
        {
            var result = "S";

            if (season > 9)
            {
                result += season.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                result += "0" + season.ToString(CultureInfo.InvariantCulture);
            }

            result += "E";

            if (episode > 9)
            {
                result += episode.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                result += "0" + episode.ToString(CultureInfo.InvariantCulture);
            }

            return result;
        }
    }
}