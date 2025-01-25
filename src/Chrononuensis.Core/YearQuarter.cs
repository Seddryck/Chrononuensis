using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;

namespace Chrononuensis;
public record struct YearQuarter(int Year, int Quarter)

    : IParsable<YearQuarter>, IComparable<YearQuarter>, IComparable, IEquatable<YearQuarter>
{
    private static YearQuarterParser Parser { get; } = new();

    public static YearQuarter Parse(string input, IFormatProvider? provider)
        => Parse(input, YearMonthParser.DefaultPattern, provider);

    public static YearQuarter Parse(string input, string format, IFormatProvider? provider = null)
    {
        var result = Parser.Parse(input, format, provider);
        return new YearQuarter(result.Year, result.Quarter);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out YearQuarter result)
        => throw new NotImplementedException();

    public readonly int CompareTo(YearQuarter other)
        => Year == other.Year ? Quarter.CompareTo(other.Quarter) : Year.CompareTo(other.Year);

    public int CompareTo(object? obj)
        => obj switch
        {
            null => 1,
            YearQuarter ym => CompareTo(ym),
            _ => throw new ArgumentException("Object must be of type YearQuarter", nameof(obj))
        };

    public static bool operator < (YearQuarter left, YearQuarter right)
        => left.CompareTo(right) < 0;

    public static bool operator >(YearQuarter left, YearQuarter right)
        => left.CompareTo(right) > 0;

    public static bool operator <=(YearQuarter left, YearQuarter right)
        => left.CompareTo(right) <= 0;

    public static bool operator >=(YearQuarter left, YearQuarter right)
        => left.CompareTo(right) >= 0;
}
