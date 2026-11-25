using System.Collections.Frozen;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers.Internals;
using Pidgin;
using Tokens = Chrononuensis.Formats.Tokens;

namespace Chrononuensis.Parsers;
internal partial class ParserFactory
{
    private static readonly FrozenDictionary<FormatToken, Parser<char, object>> Mappings = CreateMappings();
    private readonly PeriodParsingOptions _options;

    private static partial FrozenDictionary<FormatToken, Parser<char, object>> CreateMappings();

    public ParserFactory(IFormatProvider? provider = null)
        => _options = PeriodParsingOptions.From(provider);

    public Parser<char, object> Create(FormatToken token)
    {
        if (token == Tokens.Year.DigitOn2YearToken.Instance)
            return Internals.YearParser.CreateDigitOn2(_options.YearNormalizer).Cast<object>();

        if (token == Tokens.Year.RomanNumeralShortYearToken.Instance)
            return Internals.YearParser.CreateRomanNumeralShort(_options.YearNormalizer).Cast<object>();

        if (token == Tokens.Decade.DigitOn2DecadeToken.Instance)
            return Internals.DecadeParser.CreateDigitOn2(_options.DecadeNormalizer).Cast<object>();

        if (token == Tokens.Month.AbbreviationMonthToken.Instance)
            return MonthParser.CreateAbbreviation(_options.DateTimeFormat).Cast<object>();

        if (token == Tokens.Month.LabelMonthToken.Instance)
            return MonthParser.CreateLabel(_options.DateTimeFormat).Cast<object>();

        if (token is LiteralToken literal)
            return Primitives.StringParser(literal.Value).Cast<object>();

        if (token is LocalizedToken localized)
            return Primitives.LocalizedParser(localized.Key, _options.Culture).Cast<object>();

        if (token is MutuallyExclusiveToken exclusive)
            return Primitives.StringParsers(exclusive.Values.Select(x => ((LiteralToken)x).Value).ToArray()).Cast<object>();

        if (!Mappings.TryGetValue(token, out var parser))
            throw new ArgumentOutOfRangeException($"Token {token} not found in the dictionary");
        return parser;
    }
}
