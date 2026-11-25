using System.Collections.Frozen;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers.Internals;
using Pidgin;
using Tokens = Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Parsers;
internal partial class ParserFactory
{
    private static readonly FrozenDictionary<FormatToken, Mapping> Mappings = CreateMappings();
    private readonly PeriodParsingOptions _options;

    private static partial FrozenDictionary<FormatToken, Mapping> CreateMappings();

    public ParserFactory(IFormatProvider? provider = null)
        => _options = PeriodParsingOptions.From(provider);

    private readonly record struct Mapping(Parser<char, int> Value, Parser<char, object> Boxed)
    {
        internal Mapping(Parser<char, int> value) : this(value, value.Cast<object>()) { }
    }

    public Parser<char, object> Create(FormatToken token)
        => CreateNonValue(token)?.Cast<object>()
            ?? CreateProviderDependent(token)?.Cast<object>()
            ?? GetMapping(token).Boxed;

    internal Parser<char, int> CreateValue(FormatToken token)
        => CreateProviderDependent(token) ?? GetMapping(token).Value;

    internal Parser<char, Unit> CreateIgnored(FormatToken token)
        => CreateNonValue(token) ?? CreateValue(token).IgnoreResult();

    private Parser<char, int>? CreateProviderDependent(FormatToken token)
    {
        if (token == Tokens.Year.DigitOn2YearToken.Instance)
            return Internals.YearParser.CreateDigitOn2(_options.YearNormalizer);

        if (token == Tokens.Year.RomanNumeralShortYearToken.Instance)
            return Internals.YearParser.CreateRomanNumeralShort(_options.YearNormalizer);

        if (token == Tokens.Decade.DigitOn2DecadeToken.Instance)
            return Internals.DecadeParser.CreateDigitOn2(_options.DecadeNormalizer);

        if (token == Tokens.Month.AbbreviationMonthToken.Instance)
            return MonthParser.CreateAbbreviation(_options.DateTimeFormat);

        if (token == Tokens.Month.LabelMonthToken.Instance)
            return MonthParser.CreateLabel(_options.DateTimeFormat);

        return null;
    }

    private Parser<char, Unit>? CreateNonValue(FormatToken token)
    {
        if (token is LiteralToken literal)
            return Primitives.StringParser(literal.Value);

        if (token is LocalizedToken localized)
            return Primitives.LocalizedParser(localized.Key, _options.Culture);

        if (token is MutuallyExclusiveToken exclusive)
            return Primitives.StringParsers(exclusive.Values.Select(x => ((LiteralToken)x).Value).ToArray());

        return null;
    }

    private static Mapping GetMapping(FormatToken token)
    {
        if (!Mappings.TryGetValue(token, out var parser))
            throw new ArgumentOutOfRangeException($"Token {token} not found in the dictionary");
        return parser;
    }
}
