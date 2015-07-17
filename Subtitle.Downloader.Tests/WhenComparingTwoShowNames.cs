using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Downloader.Tests
{
    [TestFixture]
    public class WhenComparingTwoShowNames
    {
        private static void ShouldBeApproximateMatches(string firstString, string secondString)
        {
            AproximateCompare.With(firstString, secondString).Should().BeTrue();
            AproximateCompare.With(secondString, firstString).Should().BeTrue();
        }

        private static void ShouldNotBeApproximateMatches(string firstString, string secondString)
        {
            AproximateCompare.With(firstString, secondString).Should().BeFalse();
            AproximateCompare.With(secondString, firstString).Should().BeFalse();
        }

        [Test]
        public void CaseShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Castle", "cASTLE");
        }

        [Test]
        public void CrimeSceneInvestigationShouldMatch()
        {
            ShouldBeApproximateMatches("CSI", "CSI: Crime Scene Investigation");
            ShouldBeApproximateMatches("CSI: Crime Scene Investigation", "CSI");
        }

        [Test]
        public void NumbersShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Castle", "Castle 2009");
        }

        [Test]
        public void OneLetterWordsWhichAreDifferentShouldBeIgnored()
        {
            ShouldNotBeApproximateMatches("E", "A");
        }

        [Test]
        public void OneMissingLetterShouldBeIgnored()
        {
            ShouldBeApproximateMatches("abce", "abcde");
            ShouldBeApproximateMatches("abcd", "abcde");

            ShouldNotBeApproximateMatches("abc", "abcde");
            ShouldNotBeApproximateMatches("acce", "abcde");
        }

        [Test]
        public void OneMissingLetterShouldBeIgnoredExceptIfItIsTheFirstLetter()
        {
            ShouldNotBeApproximateMatches("bcde", "abcde");
        }

        [Test]
        public void PunctuationShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Marvel_Agents-of S.H.I.E.L.D", "Marvel Agents of SHIELD");
        }

        [Test]
        public void SpacesShouldBeIgnored()
        {
            ShouldBeApproximateMatches("castle", " c a s t l e ");
        }

        [Test]
        public void TheWordAndShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Dumbo and Dumbo", "Dumbo & Dumbo");
            ShouldBeApproximateMatches("Dumbo and Dumbo", "Dumbo @ Dumbo");
        }

        [Test]
        public void TheWordUsShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Dumbo US", "Dumbo");
        }

        [Test]
        public void TwoCompletelyDifferentWordsShouldNotMatch()
        {
            ShouldNotBeApproximateMatches("Eajsd", "Aihasdoihoi");
        }
    }
}