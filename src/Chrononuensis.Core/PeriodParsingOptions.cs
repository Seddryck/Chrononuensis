using System.Globalization;

namespace Chrononuensis;

/// <summary>
/// Defines immutable, per-operation settings used when parsing periods.
/// </summary>
public sealed class PeriodParsingOptions : IFormatProvider
{
    public IFormatProvider FormatProvider { get; }

    public Func<int, int> YearNormalizer { get; }

    public Func<int, int> DecadeNormalizer { get; }

    public PeriodParsingOptions(
        IFormatProvider? formatProvider = null,
        Func<int, int>? yearNormalizer = null,
        Func<int, int>? decadeNormalizer = null)
    {
        FormatProvider = formatProvider ?? CultureInfo.CurrentCulture;
        YearNormalizer = yearNormalizer ?? NormalizeRecentYear;
        DecadeNormalizer = decadeNormalizer ?? NormalizeRecentYear;
    }

    public object? GetFormat(Type? formatType) => FormatProvider.GetFormat(formatType);

    internal static PeriodParsingOptions From(IFormatProvider? provider)
        => provider as PeriodParsingOptions ?? new PeriodParsingOptions(provider);

    private static int NormalizeRecentYear(int value) => value < 40 ? value + 2000 : value + 1900;
}
