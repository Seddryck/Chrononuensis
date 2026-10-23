namespace Chrononuensis;

/// <summary>
/// Represents a custom, non-empty date period using canonical half-open bounds.
/// </summary>
public readonly struct CustomPeriod : IPeriod
{
    public DateOnly StartDate { get; }

    public DateOnly EndDateExclusive { get; }

    /// <summary>Creates the period <c>[startDate, endDateExclusive)</c>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="endDateExclusive"/> is on or before <paramref name="startDate"/>.
    /// Empty and reversed periods are not supported.
    /// </exception>
    public CustomPeriod(DateOnly startDate, DateOnly endDateExclusive)
    {
        if (endDateExclusive <= startDate)
            throw new ArgumentOutOfRangeException(nameof(endDateExclusive), endDateExclusive,
                $"Invalid period: exclusive end date ({endDateExclusive}) must be after start date ({startDate}).");

        StartDate = startDate;
        EndDateExclusive = endDateExclusive;
    }

    /// <summary>Creates a period from inclusive display dates.</summary>
    /// <remarks>
    /// Prefer the constructor when half-open bounds are already available. An inclusive final date of
    /// <see cref="DateOnly.MaxValue"/> cannot be represented by a <see cref="DateOnly"/> exclusive end.
    /// </remarks>
    public static CustomPeriod FromInclusiveDates(DateOnly firstDate, DateOnly lastDate)
    {
        if (lastDate < firstDate)
            throw new ArgumentOutOfRangeException(nameof(lastDate), lastDate,
                $"Invalid period: last date ({lastDate}) must be on or after first date ({firstDate}).");
        if (lastDate == DateOnly.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(lastDate), lastDate,
                "An inclusive final date of DateOnly.MaxValue has no representable exclusive end date.");

        return new CustomPeriod(firstDate, lastDate.AddDays(1));
    }

    public int Days => EndDateExclusive.DayNumber - StartDate.DayNumber;
    public DateOnly FirstDate => StartDate;
    public DateOnly LastDate => EndDateExclusive.AddDays(-1);
    public DateTime LowerBound => StartDate.ToDateTime(TimeOnly.MinValue);
    public DateTime UpperBound => EndDateExclusive.ToDateTime(TimeOnly.MinValue);

    public bool Contains(IPeriod other) =>
        StartDate <= other.StartDate && EndDateExclusive >= other.EndDateExclusive;

    public bool Overlaps(IPeriod other) =>
        StartDate < other.EndDateExclusive && EndDateExclusive > other.StartDate;

    public bool Meets(IPeriod other) =>
        EndDateExclusive == other.StartDate || other.EndDateExclusive == StartDate;

    public bool Precedes(IPeriod other) => EndDateExclusive <= other.StartDate;
    public bool Succeeds(IPeriod other) => StartDate >= other.EndDateExclusive;

    public IPeriod? Intersect(IPeriod other) =>
        Overlaps(other)
            ? new CustomPeriod(
                new[] { StartDate, other.StartDate }.Max(),
                new[] { EndDateExclusive, other.EndDateExclusive }.Min())
            : null;

    public IPeriod Span(IPeriod other) =>
        new CustomPeriod(
            new[] { StartDate, other.StartDate }.Min(),
            new[] { EndDateExclusive, other.EndDateExclusive }.Max());

    public int Gap(IPeriod other)
    {
        if (Overlaps(other) || Meets(other))
            return 0;

        return other.StartDate > StartDate
            ? other.StartDate.DayNumber - EndDateExclusive.DayNumber
            : StartDate.DayNumber - other.EndDateExclusive.DayNumber;
    }

    public override string ToString() => $"Custom period: [{StartDate}, {EndDateExclusive})";

    public bool Equals(IPeriod? other)
        => other is not null
            && StartDate == other.StartDate
            && EndDateExclusive == other.EndDateExclusive;

    public override bool Equals(object? obj) => obj is IPeriod other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(StartDate, EndDateExclusive);

    public static bool operator <(CustomPeriod left, CustomPeriod right) => left.Precedes(right);
    public static bool operator >(CustomPeriod left, CustomPeriod right) => left.Succeeds(right);
    public static bool operator <(CustomPeriod left, IPeriod right) => left.Precedes(right);
    public static bool operator >(CustomPeriod left, IPeriod right) => left.Succeeds(right);
    public static bool operator <(IPeriod left, CustomPeriod right) => left.Precedes(right);
    public static bool operator >(IPeriod left, CustomPeriod right) => left.Succeeds(right);

    private static bool IsLessThanOrEqual(IPeriod left, IPeriod right)
        => left.StartDate <= right.StartDate && left.EndDateExclusive <= right.EndDateExclusive;

    private static bool IsGreaterThanOrEqual(IPeriod left, IPeriod right)
        => left.StartDate >= right.StartDate && left.EndDateExclusive >= right.EndDateExclusive;

    public static bool operator <=(CustomPeriod left, CustomPeriod right) => IsLessThanOrEqual(left, right);
    public static bool operator >=(CustomPeriod left, CustomPeriod right) => IsGreaterThanOrEqual(left, right);
    public static bool operator <=(CustomPeriod left, IPeriod right) => IsLessThanOrEqual(left, right);
    public static bool operator >=(CustomPeriod left, IPeriod right) => IsGreaterThanOrEqual(left, right);
    public static bool operator <=(IPeriod left, CustomPeriod right) => IsLessThanOrEqual(left, right);
    public static bool operator >=(IPeriod left, CustomPeriod right) => IsGreaterThanOrEqual(left, right);

    public static bool operator ==(CustomPeriod left, CustomPeriod right) => left.Equals(right);
    public static bool operator !=(CustomPeriod left, CustomPeriod right) => !left.Equals(right);
    public static bool operator ==(CustomPeriod left, IPeriod right) => left.Equals(right);
    public static bool operator !=(CustomPeriod left, IPeriod right) => !left.Equals(right);
    public static bool operator ==(IPeriod left, CustomPeriod right) => right.Equals(left);
    public static bool operator !=(IPeriod left, CustomPeriod right) => !right.Equals(left);
}
