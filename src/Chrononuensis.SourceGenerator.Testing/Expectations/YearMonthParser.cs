#nullable enable

using Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Parsers;

public class YearMonthParser : ChrononuensisParser
{
    public static string DefaultPattern { get; } = "yyyy-MM";

    public (int Year, int Month) Parse(string input, string format, IFormatProvider? provider)
    {
        var results = ParseInternal(input, format, provider, [typeof(IYearToken), typeof(IMonthToken)]);
        return ((int)results[0], (int)results[1]);
    }

    public (int Year, int Month) Parse(ReadOnlySpan<char> input, string format, IFormatProvider? provider)
    {
        var results = ParseInternal(input, format, provider, [typeof(IYearToken), typeof(IMonthToken)]);
        return ((int)results[0], (int)results[1]);
    }

    public bool TryParse(string? input, string format, IFormatProvider? provider, out int? Year, out int? Month)
    {
        object[]? results = null;
        var result = !string.IsNullOrEmpty(input)
                        && TryParseInternal(input, format, provider, [typeof(IYearToken), typeof(IMonthToken)], out results);

        Year = result ? (int)results![0] : null;
        Month = result ? (int)results![1] : null;
        return result;
    }

    public bool TryParse(ReadOnlySpan<char> input, string format, IFormatProvider? provider, out int? Year, out int? Month)
    {
        object[]? results = null;
        var result = input.Length > 0
                        && TryParseInternal(input, format, provider, [typeof(IYearToken), typeof(IMonthToken)], out results);

        Year = result ? (int)results![0] : null;
        Month = result ? (int)results![1] : null;
        return result;
    }
}
