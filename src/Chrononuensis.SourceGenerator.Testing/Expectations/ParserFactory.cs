using System.Collections.Frozen;
using System.Collections.Generic;
using Tokens = Chrononuensis.Formats.Tokens;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers.Internals;
using Pidgin;

namespace Chrononuensis.Parsers;

internal partial class ParserFactory
{
    private static partial FrozenDictionary<FormatToken, Parser<char, object>> CreateMappings()
    {
        var mappings = new Dictionary<FormatToken, Parser<char, object>>();
        mappings.TryAdd(Tokens.Day.DigitDayToken.Instance, Parsers.Internals.DayParser.Digit.Cast<object>());
        mappings.TryAdd(Tokens.Day.PaddedDigitDayToken.Instance, Parsers.Internals.DayParser.PaddedDigit.Cast<object>());

        mappings.TryAdd(Tokens.DayOfYear.DigitDayOfYearToken.Instance, Parsers.Internals.DayOfYearParser.Digit.Cast<object>());
        mappings.TryAdd(Tokens.DayOfYear.PaddedDigitDayOfYearToken.Instance, Parsers.Internals.DayOfYearParser.PaddedDigit.Cast<object>());

        return mappings.ToFrozenDictionary();
    }
}
