#nullable enable

using Chrononuensis.Parsers;

namespace Chrononuensis;

public partial record struct YearMonth
(
    int Year,
    int Month
)
    : IParsable<YearMonth>, IComparable<YearMonth>, IComparable, IEquatable<YearMonth>
{
    public int Month { get; }
        = (Month >= 1 && Month <= 12)
            ? Month
            : throw new ArgumentOutOfRangeException(nameof(Month), "Month must be between 1 and 12.");

    private static YearMonthParser Parser { get; } = new();

    public static YearMonth Parse(string input, IFormatProvider? provider)
        => Parse(input, YearMonthParser.DefaultPattern, provider);

    public static YearMonth Parse(string input, string format, IFormatProvider? provider = null)
    {
        var result = Parser.Parse(input, format, provider);
        return new YearMonth(result.Year, result.Month);
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out YearMonth result)
        => TryParse(s, YearMonthParser.DefaultPattern, provider, out result);

    public static bool TryParse(string? s, string format, IFormatProvider? provider, out YearMonth result)
    {
        var success = Parser.TryParse(s, format, provider, out var Year, out var Month);
        result = success
                    ? new YearMonth(Year!.Value, Month!.Value)
                    : default;
        return success;
    }

    public int CompareTo(YearMonth other)
        => Year == other.Year ? Month.CompareTo(other.Month) : Year.CompareTo(other.Year);

    public int CompareTo(object? obj)
        => obj switch
        {
            null => 1,
            YearMonth y => CompareTo(y),
            _ => throw new ArgumentException("Object must be of type YearMonth", nameof(obj))
        };

    public static bool operator <(YearMonth left, YearMonth right) => left.CompareTo(right) < 0;
    public static bool operator >(YearMonth left, YearMonth right) => left.CompareTo(right) > 0;
    public static bool operator <=(YearMonth left, YearMonth right) => left.CompareTo(right) <= 0;
    public static bool operator >=(YearMonth left, YearMonth right) => left.CompareTo(right) >= 0;
}
