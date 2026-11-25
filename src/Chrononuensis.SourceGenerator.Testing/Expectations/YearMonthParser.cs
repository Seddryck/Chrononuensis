#nullable enable

using Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Parsers;

public class YearMonthParser : ChrononuensisParser
{
    private static readonly TypedPeriodParser<(int Year, int Month)> Parser = new([
        new(typeof(IYearToken), static (result, value) => (value, result.Month)),
        new(typeof(IMonthToken), static (result, value) => (result.Year, value)),
    ]);

    public static string DefaultPattern { get; } = "yyyy-MM";

    public (int Year, int Month) Parse(string input, string format, IFormatProvider? provider)
        => Parser.Parse(input, format, provider);

    public (int Year, int Month) Parse(ReadOnlySpan<char> input, string format, IFormatProvider? provider)
        => Parser.Parse(input, format, provider);

    public (int Year, int Month) Parse(ReadOnlySpan<byte> input, string format, IFormatProvider? provider)
        => Parser.Parse(input, format, provider);

    public bool TryParse(string? input, string format, IFormatProvider? provider, out int? Year, out int? Month)
    {
        (int Year, int Month) value = default;
        var success = !string.IsNullOrEmpty(input)
            && Parser.TryParse(input, format, provider, out value);

        Year = success ? value.Year : null;
        Month = success ? value.Month : null;
        return success;
    }

    public bool TryParse(ReadOnlySpan<char> input, string format, IFormatProvider? provider, out int? Year, out int? Month)
    {
        (int Year, int Month) value = default;
        var success = !input.IsEmpty
            && Parser.TryParse(input, format, provider, out value);

        Year = success ? value.Year : null;
        Month = success ? value.Month : null;
        return success;
    }

    public bool TryParse(ReadOnlySpan<byte> input, string format, IFormatProvider? provider, out int? Year, out int? Month)
    {
        (int Year, int Month) value = default;
        var success = !input.IsEmpty
            && Parser.TryParse(input, format, provider, out value);

        Year = success ? value.Year : null;
        Month = success ? value.Month : null;
        return success;
    }
}
