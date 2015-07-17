using System;
using FluentAssertions;
using NUnit.Framework;

namespace Subtitle.Downloader.Tests
{
    [TestFixture]
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

        public TimeSpan OneMinute
        {
            get { return new TimeSpan(0, 0, 1, 0); }
        }

        [Test]
        public void CurrentTimeIsBeforeScheduleWeShouldNotRun()
        {
            var currentTime = TimeCheck.TodayAt(17, 30);
            var scheduledTime = TimeCheck.TodayAt(18, 00);
            var lastRunTime = TimeCheck.TodayAt(19, 00).AddDays(-1);

            TimeCheck.ForTime(scheduledTime, lastRunTime, currentTime)
                .Should().BeFalse();
        }

        [Test]
        public void LastRunTimeIsBeforeScheduleCurrentTimeIsAfter()
        {
            var currentTime = TimeCheck.TodayAt(19, 00);
            var scheduledTime = TimeCheck.TodayAt(18, 00);
            var lastRunTime = TimeCheck.TodayAt(17, 00);
            TimeCheck.ForTime(scheduledTime, lastRunTime, currentTime)
                .Should().BeTrue();
        }

        [Test]
        public void OneMinuteHasPassedWeShouldNotRun()
        {
            TimeCheck.ForInterval(FifteenMinutes, OneMinuteAgo).Should().BeFalse();
        }

        [Test]
        public void TwentyMinutesHavePassedWeShouldRun()
        {
            TimeCheck.ForInterval(FifteenMinutes, TwentyMinutesAgo).Should().BeTrue();
        }
    }
}