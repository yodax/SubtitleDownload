using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Subtitle.Downloader
{
    public static class AproximateCompare
    {
        public static bool With(string firstString, string secondString)
        {
            var cleandFirstString = firstString.ToLower();
            var cleanedSecondString = secondString.ToLower();

            if (cleandFirstString.Contains("crime scene investigation"))
                cleandFirstString = "csi";

            if (cleanedSecondString.Contains("crime scene investigation"))
                cleanedSecondString = "csi";

            cleandFirstString = RemoveWords(cleandFirstString);
            cleanedSecondString = RemoveWords(cleanedSecondString);

            cleandFirstString = ReturnOnlyCharachters(cleandFirstString);
            cleanedSecondString = ReturnOnlyCharachters(cleanedSecondString);

            if (OnlyOneCharacterLengthDifference(firstString, secondString))
            {
                return CheckIfOnlyOneLetterIsMissing(firstString, secondString);
            }

            return cleandFirstString.Equals(cleanedSecondString);
        }

        private static bool CheckIfOnlyOneLetterIsMissing(string firstString, string secondString)
        {
            var shortString = firstString.Length < secondString.Length ? firstString : secondString;
            var longString = firstString.Length > secondString.Length ? firstString : secondString;
            var skipped = false;
            for (var i = 0; i < longString.Length; i++)
            {
                if (shortString.Length > i && shortString[i] != longString[i] && !skipped)
                {
                    // Dont check if it is the first letter
                    if (i == 0)
                        return false;

                    skipped = true;
                }
                else if (skipped && shortString[i - 1] != longString[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool OnlyOneCharacterLengthDifference(string firstString, string secondString)
        {
            return Math.Abs(firstString.Length - secondString.Length) == 1;
        }

        private static string RemoveWords(string textToSearch)
        {
            return Regex.Replace(textToSearch, @"\w+", CheckForWordsToIgnore);
        }

        private static string CheckForWordsToIgnore(Match match)
        {
            var wordsToIgnore = new[] {"and", "us"};

            if (wordsToIgnore.Any(w => w.Equals(match.Value)))
                return "";

            return match.Value;
        }

        private static string ReturnOnlyCharachters(string stringToConvert)
        {
            var result = "";
            foreach (Match match in Regex.Matches(stringToConvert.ToLower(), "[a-z]*"))
            {
                result += match.Value;
            }

            return result;
        }
    }
}