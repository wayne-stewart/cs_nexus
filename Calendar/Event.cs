using System;

namespace Calendar
{
    public class Event
    {
        public DateTime Due { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime Completed { get; set; }

        public virtual Event Clone()
        {
            return new Event
            {
                Due = Due,
                Duration = Duration,
                Completed = Completed
            };
        }
    }
}
