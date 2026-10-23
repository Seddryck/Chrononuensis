namespace Chrononuensis;

/// <summary>
/// Represents a non-empty range of whole dates using canonical half-open bounds.
/// Equality is based on <see cref="StartDate"/> and <see cref="EndDateExclusive"/>,
/// regardless of the concrete implementation.
/// </summary>
/// <remarks>
/// A period contains every date <c>d</c> for which
/// <c>StartDate &lt;= d &lt; EndDateExclusive</c>. Empty periods are not supported.
/// </remarks>
public interface IPeriod : IEquatable<IPeriod>
{
    /// <summary>Gets the inclusive start date of the period.</summary>
    DateOnly StartDate { get; }

    /// <summary>Gets the exclusive end date of the period.</summary>
    DateOnly EndDateExclusive { get; }

    /// <summary>Gets the total number of dates within the period.</summary>
    int Days { get; }

    /// <summary>Gets the inclusive start date. This compatibility alias is equivalent to <see cref="StartDate"/>.</summary>
    DateOnly FirstDate { get; }

    /// <summary>Gets the inclusive final date. This compatibility value is the day before <see cref="EndDateExclusive"/>.</summary>
    DateOnly LastDate { get; }

    /// <summary>Gets the start date as a midnight <see cref="DateTime"/> compatibility value.</summary>
    DateTime LowerBound { get; }

    /// <summary>Gets the exclusive end date as a midnight <see cref="DateTime"/> compatibility value.</summary>
    DateTime UpperBound { get; }

    bool Contains(IPeriod other);
    bool Overlaps(IPeriod other);
    bool Meets(IPeriod other);
    bool Precedes(IPeriod other);
    bool Succeeds(IPeriod other);
    IPeriod? Intersect(IPeriod other);
    IPeriod Span(IPeriod other);
    int Gap(IPeriod other);
}
