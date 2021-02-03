using System;

namespace Calendar
{
    public class InvalidOrdinalException : Exception { public InvalidOrdinalException(PatternType pattern, IntervalType interval, OrdinalType ordinal) : base($"Invalid Ordinal: {ordinal} in Pattern: {pattern} for Interval: {interval}") { } }

}
