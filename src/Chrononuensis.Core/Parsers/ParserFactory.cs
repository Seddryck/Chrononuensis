using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chrononuensis.Formats;
using Tokens = Chrononuensis.Formats.Tokens;
using Pidgin;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers.Internals;

namespace Chrononuensis.Parsers;
internal partial class ParserFactory
{
    private readonly PeriodParsingOptions _options;

    public Dictionary<FormatToken, Parser<char, object>> _dict { get; set; } = [];

    partial void Initialize();

    public ParserFactory(IFormatProvider? provider = null)
    {
        _options = PeriodParsingOptions.From(provider);
        Initialize();
    }

    protected void AddMapping(FormatToken token, Parser<char, object> parser)
    {
        _dict.TryAdd(token, parser);
    }

    public Parser<char, object> Create(FormatToken token)
    {
        if (token == Tokens.Year.DigitOn2YearToken.Instance)
            return Internals.YearParser.CreateDigitOn2(_options.YearNormalizer).Cast<object>();

        if (token == Tokens.Year.RomanNumeralShortYearToken.Instance)
            return Internals.YearParser.CreateRomanNumeralShort(_options.YearNormalizer).Cast<object>();

        if (token == Tokens.Decade.DigitOn2DecadeToken.Instance)
            return Internals.DecadeParser.CreateDigitOn2(_options.DecadeNormalizer).Cast<object>();

        if (token is LiteralToken literal)
            return Primitives.StringParser(literal.Value).Cast<object>();

        if (token is LocalizedToken localized)
            return Primitives.LocalizedParser(localized.Key).Cast<object>();

        if (token is MutuallyExclusiveToken exclusive)
            return Primitives.StringParsers(exclusive.Values.Select(x => ((LiteralToken)x).Value).ToArray()).Cast<object>();

        if (!_dict.TryGetValue(token, out var parser))
            throw new ArgumentOutOfRangeException($"Token {token} not found in the dictionary");
        return parser;
    }
}
