namespace Subtitle.Downloader.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WhenCheckingIfWeNeedToRunCheckSession
    {
        private TimeSpan FifteenMinutes
        {
            get { return new TimeSpan(0, 0, 15, 0); }
        }

        private DateTime OneMinuteAgo
        {
            get { return DateTime.Now.AddMinutes(-1); }
        }
        private DateTime TwentyMinutesAgo
        {
            get { return DateTime.Now.AddMinutes(-20); }
        }

        [TestMethod]
        public void OneMinuteHasPassedWeShouldNotRun()
        {
            TimeCheck.ForInterval(FifteenMinutes, OneMinuteAgo).Should().BeFalse();
        }

        [TestMethod]
        public void TwentyMinutesHavePassedWeShouldRun()
        {
            TimeCheck.ForInterval(FifteenMinutes, TwentyMinutesAgo).Should().BeTrue();
        }

        [TestMethod]
        public void LastRunTimeIsBeforeScheduleCurrentTimeIsAfter()
        {
            var currentTime = TimeCheck.TodayAt(19, 00);
            var scheduledTime = TimeCheck.TodayAt(18, 00);
            var lastRunTime = TimeCheck.TodayAt(17, 00);
            TimeCheck.ForTime(scheduledTime, lastRunTime, currentTime)
                .Should().BeTrue();
        }

        [TestMethod]
        public void CurrentTimeIsBeforeScheduleWeShouldNotRun()
        {
            var currentTime = TimeCheck.TodayAt(17, 30);
            var scheduledTime = TimeCheck.TodayAt(18, 00);
            var lastRunTime = TimeCheck.TodayAt(19, 00).AddDays(-1);

            TimeCheck.ForTime(scheduledTime, lastRunTime, currentTime)
                .Should().BeFalse();
        }

        public TimeSpan OneMinute {
            get { return new TimeSpan(0, 0, 1, 0); }
        }
    }
}