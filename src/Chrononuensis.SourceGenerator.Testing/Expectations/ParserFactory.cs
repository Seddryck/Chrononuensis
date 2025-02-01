using Tokens = Chrononuensis.Formats.Tokens;
using Chrononuensis.Parsers.Internals;

namespace Chrononuensis.Parsers;

internal partial class ParserFactory
{
    partial void Initialize()
    {
        AddMapping(Tokens.Day.DigitDayToken.Instance, DayParser.Digit.Cast<object>());
        AddMapping(Tokens.Day.PaddedDigitDayToken.Instance, DayParser.PaddedDigit.Cast<object>());

        AddMapping(Tokens.DayOfYear.DigitDayOfYearToken.Instance, DayOfYearParser.Digit.Cast<object>());
        AddMapping(Tokens.DayOfYear.PaddedDigitDayOfYearToken.Instance, DayOfYearParser.PaddedDigit.Cast<object>());
    }
}
