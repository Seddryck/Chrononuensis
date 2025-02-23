using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis;

/// <summary>
/// Represents a continuous time span with a defined start and end.
/// It represents a range rather than a single event.
/// </summary>
public interface IPeriod : IEquatable<IPeriod>
{
    /// <summary>
    /// Gets the total number of days within the period.
    /// </summary>
    int Days { get; }

    /// <summary>
    /// Gets the first date of the period.
    /// </summary>
    DateOnly FirstDate { get; }

    /// <summary>
    /// Gets the last date of the period.
    /// </summary>
    DateOnly LastDate { get; }

    /// <summary>
    /// Gets the lower bound of the period defined as a close/open interval, 
    /// typically representing the start of FirstDate.
    /// </summary>
    DateTime LowerBound { get; }

    /// <summary>
    /// Gets the upper bound of the period defined as a close/open interval, 
    /// typically representing the start of the day following the LastDate.
    /// </summary>
    DateTime UpperBound { get; }

    /// <summary>
    /// Determines if this period fully contains another period.
    /// </summary>
    bool Contains(IPeriod other);

    /// <summary>
    /// Determines if this period overlaps with another period.
    /// </summary>
    bool Overlaps(IPeriod other);

    /// <summary>
    /// Determines if this period meets another period (i.e., is adjacent without overlap).
    /// </summary>
    bool Meets(IPeriod other);

    /// <summary>
    /// Determines if this period is strictly before another.
    /// </summary>
    bool Precedes(IPeriod other);

    /// <summary>
    /// Determines if this period is strictly after another.
    /// </summary>
    bool Succeeds(IPeriod other);

    /// <summary>
    /// Returns the intersection of two periods if they overlap.
    /// </summary>
    IPeriod? Intersect(IPeriod other);

    /// <summary>
    /// Returns the span of two periods, merging them into the smallest enclosing period.
    /// </summary>
    IPeriod Span(IPeriod other);

    /// <summary>
    /// Determines the gap (number of days) between two non-overlapping periods.
    /// </summary>
    int Gap(IPeriod other);
}
