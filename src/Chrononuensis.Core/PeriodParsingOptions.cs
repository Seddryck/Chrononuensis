using System.Globalization;

namespace Chrononuensis;

/// <summary>
/// Defines immutable, per-operation settings used when parsing periods.
/// </summary>
public sealed class PeriodParsingOptions : IFormatProvider
{
    /// <summary>
    /// Gets the provider used for culture-sensitive parsing. When no provider is supplied,
    /// <see cref="CultureInfo.CurrentUICulture"/> is captured for the parse operation.
    /// </summary>
    public IFormatProvider FormatProvider { get; }

    public Func<int, int> YearNormalizer { get; }

    public Func<int, int> DecadeNormalizer { get; }

    internal CultureInfo Culture { get; }

    internal DateTimeFormatInfo DateTimeFormat { get; }

    public PeriodParsingOptions(
        IFormatProvider? formatProvider = null,
        Func<int, int>? yearNormalizer = null,
        Func<int, int>? decadeNormalizer = null)
    {
        FormatProvider = formatProvider ?? CultureInfo.CurrentUICulture;
        YearNormalizer = yearNormalizer ?? NormalizeRecentYear;
        DecadeNormalizer = decadeNormalizer ?? NormalizeRecentYear;
        Culture = ResolveCulture(FormatProvider);
        DateTimeFormat = DateTimeFormatInfo.GetInstance(FormatProvider);
    }

    public object? GetFormat(Type? formatType) => FormatProvider.GetFormat(formatType);

    internal static PeriodParsingOptions From(IFormatProvider? provider)
        => provider as PeriodParsingOptions ?? new PeriodParsingOptions(provider);

    private static int NormalizeRecentYear(int value) => value < 40 ? value + 2000 : value + 1900;

    private static CultureInfo ResolveCulture(IFormatProvider provider)
        => provider as CultureInfo
            ?? provider.GetFormat(typeof(CultureInfo)) as CultureInfo
            ?? CultureInfo.CurrentCulture;
}
