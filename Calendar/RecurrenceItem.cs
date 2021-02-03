using System;

namespace Calendar
{
    internal struct RecurrenceItem
    {
        public RecurrenceItem(DayOfWeek day_of_week, bool selected)
        {
            DayOfWeek = day_of_week;
            Selected = selected;
        }
        public DayOfWeek DayOfWeek;
        public bool Selected;
    }
}
