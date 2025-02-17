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
public interface IPeriod
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
}
