using System;

namespace Calendar
{
    public class NotCompletedException : Exception { public NotCompletedException() : base("Cannot calculate Floating recurrance when record is not complete!") { } }

}
