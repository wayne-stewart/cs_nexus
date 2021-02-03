using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Calendar
{
    public class Recurrence
    {
        public DateTime End { get; set; }
        public int Frequency { get; set; }
        public OrdinalType Ordinal { get; set; }
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public IntervalType Interval { get; set; }
        public PatternType Pattern { get; set; }

        private bool FindNextDayOfCurrentWeek(DayOfWeek current, out DayOfWeek next)
        {
            foreach (var day_of_week in EnumerateDaysOfWeek())
            {
                if (day_of_week.DayOfWeek > current && day_of_week.Selected)
                {
                    next = day_of_week.DayOfWeek;
                    return true;
                }
            }
            next = DayOfWeek.Sunday;
            return false;
        }
        private DateTime FindNextOrdinalDateInMonth(DateTime date, DayOfWeek day_of_week, OrdinalType ordinal)
        {
            switch (ordinal)
            {
                case OrdinalType.First:
                case OrdinalType.Second:
                case OrdinalType.Third:
                case OrdinalType.Fourth:
                    for (var i = 0; i < 7; i++)
                    {
                        if (date.DayOfWeek == day_of_week)
                        {
                            date = date.AddDays((int)Ordinal);
                            break;
                        }
                        date = date.AddDays(1);
                    }
                    break;
                case OrdinalType.Final:
                    date = date.AddMonths(1).AddDays(-1);
                    break;
                default:
                    throw new InvalidOrdinalException(Pattern, Interval, Ordinal);
            }
            return date;
        }
        private IEnumerable<RecurrenceItem> EnumerateDaysOfWeek()
        {
            yield return new RecurrenceItem(DayOfWeek.Sunday, Sunday);
            yield return new RecurrenceItem(DayOfWeek.Monday, Monday);
            yield return new RecurrenceItem(DayOfWeek.Tuesday, Tuesday);
            yield return new RecurrenceItem(DayOfWeek.Wednesday, Wednesday);
            yield return new RecurrenceItem(DayOfWeek.Thursday, Thursday);
            yield return new RecurrenceItem(DayOfWeek.Friday, Friday);
            yield return new RecurrenceItem(DayOfWeek.Saturday, Saturday);
        }
        private DateTime CreateDateTime(DateTime date, DateTime time) => new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, DateTimeKind.Unspecified);

        public Event Next(Event record)
        {
            var new_record = record.Clone();
            new_record.Completed = DateTime.MinValue;
            switch (Interval)
            {
                case IntervalType.Seconds:
                    switch(Pattern)
                    {
                        case PatternType.Floating:
                            if (record.Completed == DateTime.MinValue) throw new NotCompletedException();
                            new_record.Due = record.Completed.AddSeconds(Frequency);
                            break;
                        case PatternType.Fixed:
                            new_record.Due = record.Due.AddSeconds(Frequency);
                            break;
                        default:
                            throw new InvalidPatternException(Pattern, Interval);
                    }
                    break;
                case IntervalType.Daily:
                    switch(Pattern)
                    {
                        case PatternType.Floating:
                            if (record.Completed == DateTime.MinValue) throw new NotCompletedException();
                            new_record.Due = CreateDateTime(record.Completed.AddDays(Frequency), record.Due);
                            break;
                        case PatternType.Fixed:
                            new_record.Due = CreateDateTime(record.Due.AddDays(Frequency), record.Due);
                            break;
                        default:
                            throw new InvalidPatternException(Pattern, Interval);
                    }
                    break;
                case IntervalType.Weekly:
                    switch (Pattern)
                    {
                        case PatternType.DaysOfWeek:
                            var start_of_week = record.Due.AddDays(-(int)record.Due.DayOfWeek);
                            DayOfWeek next_day_of_week;
                            if (!FindNextDayOfCurrentWeek(record.Due.DayOfWeek, out next_day_of_week))
                            {
                                start_of_week = start_of_week.AddDays(7 * Frequency);
                                next_day_of_week = EnumerateDaysOfWeek().First(x => x.Selected).DayOfWeek;
                            }
                            new_record.Due = CreateDateTime(start_of_week.AddDays((int)next_day_of_week), record.Due);
                            break;
                        default:
                            throw new InvalidPatternException(Pattern, Interval);
                    }
                    break;
                case IntervalType.Monthly:
                    switch (Pattern)
                    {
                        case PatternType.Floating:
                            if (record.Completed == DateTime.MinValue) throw new NotCompletedException();
                            new_record.Due = CreateDateTime(record.Completed.AddMonths(Frequency), record.Due);
                            break;
                        case PatternType.Fixed:
                            new_record.Due = CreateDateTime(record.Due.AddMonths(Frequency), record.Due);
                            break;
                        case PatternType.Ordinal:
                            // start at the first day of the month
                            new_record.Due = record.Due.AddMonths(Frequency).AddDays(-(record.Due.Day - 1));
                            new_record.Due = FindNextOrdinalDateInMonth(new_record.Due, record.Due.DayOfWeek, Ordinal);
                            new_record.Due = CreateDateTime(new_record.Due, record.Due);
                            break;
                        default:
                            throw new InvalidPatternException(Pattern, Interval);
                    }
                    break;
                case IntervalType.Yearly:
                    switch (Pattern)
                    {
                        case PatternType.Floating:
                            if (record.Completed == DateTime.MinValue) throw new NotCompletedException();
                            new_record.Due = CreateDateTime(record.Completed.AddYears(Frequency), record.Due);
                            break;
                        case PatternType.Fixed:
                            new_record.Due = CreateDateTime(record.Due.AddYears(Frequency), record.Due);
                            break;
                        case PatternType.Ordinal:
                            // start at the first day of the month
                            new_record.Due = record.Due.AddYears(Frequency).AddDays(-(record.Due.Day - 1));
                            new_record.Due = FindNextOrdinalDateInMonth(new_record.Due, record.Due.DayOfWeek, Ordinal);
                            new_record.Due = CreateDateTime(new_record.Due, record.Due);
                            break;
                        default:
                            throw new InvalidPatternException(Pattern, Interval);
                    }
                    break;
            }
            return new_record;
        }
        public bool HasEnded(Event record) => record.Due > End;
        public virtual Recurrence Clone()
        {
            return new Recurrence
            {
                End = End,
                Frequency = Frequency,
                Ordinal = Ordinal,
                Sunday = Sunday,
                Monday = Monday,
                Tuesday = Tuesday,
                Wednesday = Wednesday,
                Thursday = Thursday,
                Friday = Friday,
                Saturday = Saturday,
                Interval = Interval,
                Pattern = Pattern
            };
        }
    }
}
