using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrononuensis;

/// <summary>
/// Represents a calendar year.
/// </summary>
public partial record struct Year
{
    /// <summary>
    /// Determines whether the current year is a leap year.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the year is a leap year; otherwise, <c>false</c>.
    /// </returns>
    public bool IsLeapYear()
        => DateTime.IsLeapYear(Value);
}

