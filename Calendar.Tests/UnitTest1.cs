using NUnit.Framework;
using System;

namespace Calendar.Tests
{
    public class Tests
    {
        [TestCase("1/1/2021 10:00 AM", 1 * 60, "1/1/2021 10:01 AM")]
        [TestCase("1/1/2021 10:00 AM", 2 * 60, "1/1/2021 10:02 AM")]
        [TestCase("1/1/2021 10:00 AM", 1 * 60 * 60, "1/1/2021 11:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 7 * 60 * 60, "1/1/2021 5:00 PM")]
        public void Seconds_Fixed(string str_start_date, int frequency, string str_next_date)
        {
            var start_date = DateTime.Parse(str_start_date);
            var next_date = DateTime.Parse(str_next_date);
            var start_event = new Event { Due = start_date, Duration = new TimeSpan(1, 0, 0) };
            var sut = new Recurrence
            {
                Interval = IntervalType.Seconds,
                Pattern = PatternType.Fixed,
                Frequency = frequency
            };
            var next_event = sut.Next(start_event);
            Assert.AreEqual(next_date, next_event.Due);
        }

        [TestCase("1/1/2021 10:00 AM", 1 * 60, 1 * 60, "1/1/2021 10:02 AM")]
        [TestCase("1/1/2021 10:00 AM", 2 * 60, 2 * 60, "1/1/2021 10:04 AM")]
        [TestCase("1/1/2021 10:00 AM", 1 * 60, 1.5 * 60 * 60, "1/1/2021 11:31 AM")]
        public void Seconds_Floating(string str_start_date, int frequency, double seconds_delay, string str_next_date)
        {
            var start_date = DateTime.Parse(str_start_date);
            var next_date = DateTime.Parse(str_next_date);
            var start_event = new Event { Due = start_date, Duration = new TimeSpan(1, 0, 0), Completed = start_date.AddSeconds(seconds_delay) };
            var sut = new Recurrence
            {
                Interval = IntervalType.Seconds,
                Pattern = PatternType.Floating,
                Frequency = frequency
            };
            var next_event = sut.Next(start_event);
            Assert.AreEqual(next_date, next_event.Due);
        }

        [TestCase("1/1/2021 10:00 AM", 1, "1/2/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 2, "1/3/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 3, "1/4/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 30, "1/31/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 31, "2/1/2021 10:00 AM")]
        public void Daily_Fixed(string str_start_date, int frequency, string str_next_date)
        {
            var start_date = DateTime.Parse(str_start_date);
            var next_date = DateTime.Parse(str_next_date);
            var start_event = new Event { Due = start_date, Duration = new TimeSpan(1,0,0) };
            var sut = new Recurrence 
            {
                Interval = IntervalType.Daily,
                Pattern = PatternType.Fixed,
                Frequency = frequency
            };
            var next_event = sut.Next(start_event);
            Assert.AreEqual(next_date, next_event.Due);
        }

        [TestCase("1/1/2021 10:00 AM", 1, 0, "1/2/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 1, 1, "1/3/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 2, 3, "1/6/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 1, 1.3, "1/3/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 1, 1.5, "1/3/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 1, 1.999, "1/4/2021 10:00 AM")]
        public void Daily_Floating(string str_start_date, int frequency, double days_delay, string str_next_date)
        {
            var start_date = DateTime.Parse(str_start_date);
            var next_date = DateTime.Parse(str_next_date);
            var start_event = new Event { Due = start_date, Duration = new TimeSpan(1, 0, 0), Completed = start_date.AddDays(days_delay) };
            var sut = new Recurrence
            {
                Interval = IntervalType.Daily,
                Pattern = PatternType.Floating,
                Frequency = frequency
            };
            var next_event = sut.Next(start_event);
            Assert.AreEqual(next_date, next_event.Due);
        }

        // friday
        [TestCase("1/1/2021 10:00 AM", 1, false, false, false, false, false, true, false, "1/8/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 2, false, false, false, false, false, true, false, "1/15/2021 10:00 AM")]

        // saturday
        [TestCase("1/2/2021 10:00 AM", 1, false, false, false, false, false, false, true, "1/9/2021 10:00 AM")]
        [TestCase("1/2/2021 10:00 AM", 2, false, false, false, false, false, false, true, "1/16/2021 10:00 AM")]

        // sunday
        [TestCase("1/3/2021 10:00 AM", 1, true, false, false, false, false, false, false, "1/10/2021 10:00 AM")]
        [TestCase("1/3/2021 10:00 AM", 2, true, false, false, false, false, false, false, "1/17/2021 10:00 AM")]

        // monday
        [TestCase("1/4/2021 10:00 AM", 1, false, true, false, false, false, false, false, "1/11/2021 10:00 AM")]
        [TestCase("1/4/2021 10:00 AM", 2, false, true, false, false, false, false, false, "1/18/2021 10:00 AM")]

        // tuesday
        [TestCase("1/5/2021 10:00 AM", 1, false, false, true, false, false, false, false, "1/12/2021 10:00 AM")]
        [TestCase("1/5/2021 10:00 AM", 2, false, false, true, false, false, false, false, "1/19/2021 10:00 AM")]

        // wednesday
        [TestCase("1/6/2021 10:00 AM", 1, false, false, false, true, false, false, false, "1/13/2021 10:00 AM")]
        [TestCase("1/6/2021 10:00 AM", 2, false, false, false, true, false, false, false, "1/20/2021 10:00 AM")]

        // thursday
        [TestCase("1/7/2021 10:00 AM", 1, false, false, false, false, true, false, false, "1/14/2021 10:00 AM")]
        [TestCase("1/7/2021 10:00 AM", 2, false, false, false, false, true, false, false, "1/21/2021 10:00 AM")]

        // weekends
        [TestCase("1/02/2021 10:00 AM", 1, true, false, false, false, false, false, true, "1/03/2021 10:00 AM")]
        [TestCase("1/03/2021 10:00 AM", 1, true, false, false, false, false, false, true, "1/09/2021 10:00 AM")]
        [TestCase("1/09/2021 10:00 AM", 1, true, false, false, false, false, false, true, "1/10/2021 10:00 AM")]
        [TestCase("1/10/2021 10:00 AM", 1, true, false, false, false, false, false, true, "1/16/2021 10:00 AM")]
        [TestCase("1/02/2021 10:00 AM", 2, true, false, false, false, false, false, true, "1/10/2021 10:00 AM")]
        [TestCase("1/10/2021 10:00 AM", 2, true, false, false, false, false, false, true, "1/16/2021 10:00 AM")]
        [TestCase("1/16/2021 10:00 AM", 2, true, false, false, false, false, false, true, "1/24/2021 10:00 AM")]
        [TestCase("1/24/2021 10:00 AM", 2, true, false, false, false, false, false, true, "1/30/2021 10:00 AM")]

        // weekdays
        [TestCase("1/04/2021 10:00 AM", 1, false, true, true, true, true, true, false, "1/05/2021 10:00 AM")]
        [TestCase("1/05/2021 10:00 AM", 1, false, true, true, true, true, true, false, "1/06/2021 10:00 AM")]
        [TestCase("1/06/2021 10:00 AM", 1, false, true, true, true, true, true, false, "1/07/2021 10:00 AM")]
        [TestCase("1/07/2021 10:00 AM", 1, false, true, true, true, true, true, false, "1/08/2021 10:00 AM")]
        [TestCase("1/08/2021 10:00 AM", 1, false, true, true, true, true, true, false, "1/11/2021 10:00 AM")]
        [TestCase("1/11/2021 10:00 AM", 1, false, true, true, true, true, true, false, "1/12/2021 10:00 AM")]
        [TestCase("1/04/2021 10:00 AM", 2, false, true, true, true, true, true, false, "1/05/2021 10:00 AM")]
        [TestCase("1/05/2021 10:00 AM", 2, false, true, true, true, true, true, false, "1/06/2021 10:00 AM")]
        [TestCase("1/06/2021 10:00 AM", 2, false, true, true, true, true, true, false, "1/07/2021 10:00 AM")]
        [TestCase("1/07/2021 10:00 AM", 2, false, true, true, true, true, true, false, "1/08/2021 10:00 AM")]
        [TestCase("1/08/2021 10:00 AM", 2, false, true, true, true, true, true, false, "1/18/2021 10:00 AM")]
        [TestCase("1/18/2021 10:00 AM", 2, false, true, true, true, true, true, false, "1/19/2021 10:00 AM")]
        [TestCase("1/19/2021 10:00 AM", 2, false, true, true, true, true, true, false, "1/20/2021 10:00 AM")]
        [TestCase("1/20/2021 10:00 AM", 2, false, true, true, true, true, true, false, "1/21/2021 10:00 AM")]
        [TestCase("1/21/2021 10:00 AM", 2, false, true, true, true, true, true, false, "1/22/2021 10:00 AM")]
        [TestCase("1/22/2021 10:00 AM", 2, false, true, true, true, true, true, false, "2/01/2021 10:00 AM")]
        [TestCase("2/01/2021 10:00 AM", 2, false, true, true, true, true, true, false, "2/02/2021 10:00 AM")]
        [TestCase("2/02/2021 10:00 AM", 2, false, true, true, true, true, true, false, "2/03/2021 10:00 AM")]
        public void Weekly_DaysOfWeek(string str_start_date, int frequency, bool sun, bool mon, bool tue, bool wed, bool thr, bool fri, bool sat, string str_next_date)
        {
            var start_date = DateTime.Parse(str_start_date);
            var next_date = DateTime.Parse(str_next_date);
            var start_event = new Event { Due = start_date, Duration = new TimeSpan(1, 0, 0) };
            var sut = new Recurrence
            {
                Interval = IntervalType.Weekly,
                Pattern = PatternType.DaysOfWeek,
                Frequency = frequency,
                Sunday = sun,
                Monday = mon,
                Tuesday = tue,
                Wednesday = wed,
                Thursday = thr,
                Friday = fri,
                Saturday = sat
            };
            var next_event = sut.Next(start_event);
            Assert.AreEqual(next_date, next_event.Due);
        }

        [TestCase("1/1/2021 10:00 AM", 1, "2/1/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 2, "3/1/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 3, "4/1/2021 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 12, "1/1/2022 10:00 AM")]
        [TestCase("1/1/2021 10:00 AM", 13, "2/1/2022 10:00 AM")]
        public void Monthly_Fixed(string str_start_date, int frequency, string str_next_date)
        {
            var start_date = DateTime.Parse(str_start_date);
            var next_date = DateTime.Parse(str_next_date);
            var start_event = new Event { Due = start_date, Duration = new TimeSpan(1, 0, 0) };
            var sut = new Recurrence
            {
                Interval = IntervalType.Monthly,
                Pattern = PatternType.Fixed,
                Frequency = frequency
            };
            var next_event = sut.Next(start_event);
            Assert.AreEqual(next_date, next_event.Due);
        }
    }
}