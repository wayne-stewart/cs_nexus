using System;

namespace Calendar
{
    public class InvalidPatternException : Exception { public InvalidPatternException(PatternType pattern, IntervalType interval) : base($"Invalid Pattern: {pattern} for Interval : {interval}") { } }

}
