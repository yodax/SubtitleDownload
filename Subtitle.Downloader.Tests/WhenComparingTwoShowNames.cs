namespace Subtitle.Downloader.Tests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenComparingTwoShowNames
    {
        [TestMethod]
        public void CrimeSceneInvestigationShouldMatch()
        {
            ShouldBeApproximateMatches("CSI", "CSI: Crime Scene Investigation");
            ShouldBeApproximateMatches("CSI: Crime Scene Investigation", "CSI");
        }

        [TestMethod]
        public void OneMissingLetterShouldBeIgnored()
        {
            ShouldBeApproximateMatches("abce", "abcde");
            ShouldBeApproximateMatches("abcd", "abcde");

            ShouldNotBeApproximateMatches("abc", "abcde");
            ShouldNotBeApproximateMatches("acce", "abcde");

        }

        [TestMethod]
        public void OneMissingLetterShouldBeIgnoredExceptIfItIsTheFirstLetter()
        {
            ShouldNotBeApproximateMatches("bcde", "abcde");
        }

        [TestMethod]
        public void OneLetterWordsWhichAreDifferentShouldBeIgnored()
        {
            ShouldNotBeApproximateMatches("E", "A");
        }

        [TestMethod]
        public void TwoCompletelyDifferentWordsShouldNotMatch()
        {
            ShouldNotBeApproximateMatches("Eajsd", "Aihasdoihoi");
        }

        [TestMethod]
        public void NumbersShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Castle", "Castle 2009");
        }

        [TestMethod]
        public void CaseShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Castle", "cASTLE");
        }

        [TestMethod]
        public void SpacesShouldBeIgnored()
        {
            ShouldBeApproximateMatches("castle", " c a s t l e ");
        }

        [TestMethod]
        public void PunctuationShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Marvel_Agents-of S.H.I.E.L.D", "Marvel Agents of SHIELD");
        }

        [TestMethod]
        public void TheWordAndShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Dumbo and Dumbo", "Dumbo & Dumbo");
            ShouldBeApproximateMatches("Dumbo and Dumbo", "Dumbo @ Dumbo");

        }

        [TestMethod]
        public void TheWordUsShouldBeIgnored()
        {
            ShouldBeApproximateMatches("Dumbo US", "Dumbo");
        }

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
    }
}