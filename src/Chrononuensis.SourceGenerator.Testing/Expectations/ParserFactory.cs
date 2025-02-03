using Tokens = Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers.Internals;

namespace Chrononuensis.Parsers;

internal partial class ParserFactory
{
    partial void Initialize()
    {
        AddMapping(Tokens.Day.DigitDayToken.Instance, Parsers.Internals.DayParser.Digit.Cast<object>());
        AddMapping(Tokens.Day.PaddedDigitDayToken.Instance, Parsers.Internals.DayParser.PaddedDigit.Cast<object>());

        AddMapping(Tokens.DayOfYear.DigitDayOfYearToken.Instance, Parsers.Internals.DayOfYearParser.Digit.Cast<object>());
        AddMapping(Tokens.DayOfYear.PaddedDigitDayOfYearToken.Instance, Parsers.Internals.DayOfYearParser.PaddedDigit.Cast<object>());
    }
}
