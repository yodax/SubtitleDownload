using System;

namespace Subtitle.Downloader
{
    public class TimeCheck
    {
        public static bool ForInterval(TimeSpan timeThatShouldHavePassed, DateTime lastRunTime)
        {
            return (lastRunTime + timeThatShouldHavePassed) <= DateTime.Now;
        }

        public static bool ForTime(DateTime scheduledTime, DateTime lastRunTime, DateTime? currentTime = null)
        {
            var now = !currentTime.HasValue ? DateTime.Now : currentTime.Value;

            return now >= scheduledTime && lastRunTime < scheduledTime;
        }

        public static DateTime TodayAt(int hours, int minutes)
        {
            return DateTime.Today.AddHours(hours).AddMinutes(minutes);
        }
    }
}