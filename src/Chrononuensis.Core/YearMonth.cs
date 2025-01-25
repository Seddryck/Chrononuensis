using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Parsers;

namespace Chrononuensis;
public record struct YearMonth(int Year, int Month)

    : IParsable<YearMonth>, IComparable<YearMonth>, IComparable, IEquatable<YearMonth>
{
    private static YearMonthParser Parser { get; } = new();

    public static YearMonth Parse(string input, IFormatProvider? provider)
        => Parse(input, YearMonthParser.DefaultPattern, provider);

    public static YearMonth Parse(string input, string format, IFormatProvider? provider = null)
    {
        var result = Parser.Parse(input, format, provider);
        return new YearMonth(result.Year, result.Month);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out YearMonth result)
        => throw new NotImplementedException();

    public readonly int CompareTo(YearMonth other)
        => Year == other.Year ? Month.CompareTo(other.Month) : Year.CompareTo(other.Year);

    public int CompareTo(object? obj)
        => obj switch
        {
            null => 1,
            YearMonth ym => CompareTo(ym),
            _ => throw new ArgumentException("Object must be of type YearMonth", nameof(obj))
        };

    public static bool operator < (YearMonth left, YearMonth right)
        => left.CompareTo(right) < 0;

    public static bool operator >(YearMonth left, YearMonth right)
        => left.CompareTo(right) > 0;

    public static bool operator <=(YearMonth left, YearMonth right)
        => left.CompareTo(right) <= 0;

    public static bool operator >=(YearMonth left, YearMonth right)
        => left.CompareTo(right) >= 0;
}
