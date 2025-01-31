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
internal class ParserFactory
{
    public Dictionary<FormatToken, Parser<char, object>> _dict { get; set; } = [];

    public ParserFactory()
    {
        AddMapping(Tokens.Day.DigitDayToken.Instance, DayParser.Digit.Cast<object>());
        AddMapping(Tokens.Day.PaddedDigitDayToken.Instance, DayParser.PaddedDigit.Cast<object>());

        AddMapping(Tokens.DayOfYear.DigitDayOfYearToken.Instance, DayOfYearParser.Digit.Cast<object>());
        AddMapping(Tokens.DayOfYear.PaddedDigitDayOfYearToken.Instance, DayOfYearParser.PaddedDigit.Cast<object>());

        AddMapping(Tokens.Month.AbbreviationMonthToken.Instance, MonthParser.Abbreviation.Cast<object>());
        AddMapping(Tokens.Month.LabelMonthToken.Instance, MonthParser.Label.Cast<object>());
        AddMapping(Tokens.Month.DigitMonthToken.Instance, MonthParser.Digit.Cast<object>());
        AddMapping(Tokens.Month.PaddedDigitMonthToken.Instance, MonthParser.PaddedDigit.Cast<object>());

        AddMapping(Tokens.Quarter.DigitQuarterToken.Instance, QuarterParser.OneDigit.Cast<object>());

        AddMapping(Tokens.Semester.DigitSemesterToken.Instance, SemesterParser.OneDigit.Cast<object>());

        AddMapping(Tokens.Year.DigitOn2YearToken.Instance, YearParser.TwoDigit.Cast<object>());
        AddMapping(Tokens.Year.DigitOn4YearToken.Instance, YearParser.FourDigit.Cast<object>());
    }

    private void AddMapping(FormatToken token, Parser<char, object> parser)
    {
        if (!_dict.TryAdd(token, parser))
            throw new ArgumentException($"Token {token} already exists in the dictionary");
    }

    public Parser<char, object> Create(FormatToken token)
    {
        if (token is LiteralToken literal)
            return Primitives.CharParser(literal.Value).Cast<object>();

        if (!_dict.TryGetValue(token, out var parser))
            throw new ArgumentOutOfRangeException($"Token {token} not found in the dictionary");
        return parser;
    }
}
