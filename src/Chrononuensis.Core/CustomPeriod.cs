using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis;
public readonly struct CustomPeriod : IPeriod
{
    public DateOnly FirstDate { get; init; }

    public DateOnly LastDate { get; init; }

    public CustomPeriod(DateOnly firstDate, DateOnly lastDate)
    {
        if (firstDate > lastDate)
            throw new ArgumentException($"The start date ({firstDate}) cannot be later than the end date ({lastDate}).", nameof(firstDate));
        (FirstDate, LastDate) = (firstDate, lastDate);
    }

    public int Days => FirstDate.DayNumber - LastDate.DayNumber + 1;

    public DateTime LowerBound => FirstDate.ToDateTime(TimeOnly.MinValue);

    public DateTime UpperBound => LastDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

    /// <summary>
    /// Determines if this period fully contains another period.
    /// </summary>
    public bool Contains(IPeriod other) =>
        FirstDate <= other.FirstDate && LastDate >= other.LastDate;

    /// <summary>
    /// Determines if this period overlaps with another period.
    /// </summary>
    public bool Overlaps(IPeriod other) =>
        FirstDate <= other.LastDate && LastDate >= other.FirstDate;

    /// <summary>
    /// Determines if this period meets another period (i.e., is adjacent without overlap).
    /// </summary>
    public bool Meets(IPeriod other) =>
        LastDate.AddDays(1) == other.FirstDate || other.LastDate.AddDays(1) == FirstDate;

    /// <summary>
    /// Determines if this period is strictly before another.
    /// </summary>
    public bool Precedes(IPeriod other) => LastDate < other.FirstDate;

    /// <summary>
    /// Determines if this period is strictly after another.
    /// </summary>
    public bool Succeeds(IPeriod other) => FirstDate > other.LastDate;

    /// <summary>
    /// Returns the intersection of two periods if they overlap.
    /// </summary>
    public IPeriod? Intersect(IPeriod other) =>
        Overlaps(other)
            ? new CustomPeriod(
                DateOnlyExtensions.Max(FirstDate, other.FirstDate),
                DateOnlyExtensions.Min(LastDate, other.LastDate))
            : null;

    /// <summary>
    /// Returns the span of two periods, merging them into the smallest enclosing period.
    /// </summary>
    public IPeriod Span(IPeriod other) =>
        new CustomPeriod(
            DateOnlyExtensions.Min(FirstDate, other.FirstDate),
            DateOnlyExtensions.Max(LastDate, other.LastDate));

    /// <summary>
    /// Determines the gap (number of days) between two non-overlapping periods.
    /// </summary>
    public int Gap(IPeriod other) =>
        Overlaps(other) || Meets(other) ? 0 :
        other.FirstDate > LastDate ? other.FirstDate.DayNumber - LastDate.DayNumber - 1 :
        FirstDate.DayNumber - other.LastDate.DayNumber - 1;

    public override string ToString() => $"Custom period: {FirstDate} - {LastDate}";


    public bool Equals(IPeriod? other) => other is not null && this == other;
    public override bool Equals(object? obj) => obj is IPeriod period && this == period;

    public override int GetHashCode() => HashCode.Combine(FirstDate, LastDate);

    public static bool operator <(CustomPeriod left, CustomPeriod right) => left.Precedes(right);
    public static bool operator >(CustomPeriod left, CustomPeriod right) => left.Succeeds(right);
    public static bool operator <=(CustomPeriod left, CustomPeriod right) => left.Precedes(right) || left.LastDate == right.FirstDate;
    public static bool operator >=(CustomPeriod left, CustomPeriod right) => left.Succeeds(right) || left.LastDate == right.FirstDate;
    public static bool operator <(CustomPeriod left, IPeriod right) => left.Precedes(right);
    public static bool operator >(CustomPeriod left, IPeriod right) => left.Succeeds(right);
    public static bool operator <=(CustomPeriod left, IPeriod right) => left.Precedes(right) || left.LastDate == right.FirstDate;
    public static bool operator >=(CustomPeriod left, IPeriod right) => left.Succeeds(right) || left.LastDate == right.FirstDate;
    public static bool operator <(IPeriod left, CustomPeriod right) => left.Precedes(right);
    public static bool operator >(IPeriod left, CustomPeriod right) => left.Succeeds(right);
    public static bool operator <=(IPeriod left, CustomPeriod right) => left.Precedes(right) || left.LastDate == right.FirstDate;
    public static bool operator >=(IPeriod left, CustomPeriod right) => left.Succeeds(right) || left.LastDate == right.FirstDate;

    public static bool operator ==(CustomPeriod left, IPeriod right) => left.FirstDate == right.FirstDate && left.LastDate == right.LastDate;
    public static bool operator !=(CustomPeriod left, IPeriod right) => !(left == right);
    public static bool operator ==(IPeriod left, CustomPeriod right) => left.FirstDate == right.FirstDate && left.LastDate == right.LastDate;
    public static bool operator !=(IPeriod left, CustomPeriod right) => !(left == right);
}
