using System.Collections.Frozen;
using System.Collections.Generic;
using Tokens = Chrononuensis.Formats.Tokens;
using Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers.Internals;

namespace Chrononuensis.Parsers;

internal partial class ParserFactory
{
    private static partial FrozenDictionary<FormatToken, Mapping> CreateMappings()
    {
        var mappings = new Dictionary<FormatToken, Mapping>();
        mappings.TryAdd(Tokens.Day.DigitDayToken.Instance, new Mapping(Parsers.Internals.DayParser.Digit));
        mappings.TryAdd(Tokens.Day.PaddedDigitDayToken.Instance, new Mapping(Parsers.Internals.DayParser.PaddedDigit));

        mappings.TryAdd(Tokens.DayOfYear.DigitDayOfYearToken.Instance, new Mapping(Parsers.Internals.DayOfYearParser.Digit));
        mappings.TryAdd(Tokens.DayOfYear.PaddedDigitDayOfYearToken.Instance, new Mapping(Parsers.Internals.DayOfYearParser.PaddedDigit));

        return mappings.ToFrozenDictionary();
    }
}
