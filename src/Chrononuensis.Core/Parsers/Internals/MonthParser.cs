using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers.Internals;
internal class MonthParser
{
    public static Parser<char, int> Digit { get; } = Primitives.OneOrTwoDigitParser(1, 12);
    public static Parser<char, int> PaddedDigit { get; } = Primitives.TwoDigitParser(1, 12);

    public static Parser<char, int> Abbreviation { get; }
        = Primitives.ListParser(CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames);
    public static Parser<char, int> Label { get; }
        = Primitives.ListParser(CultureInfo.InvariantCulture.DateTimeFormat.MonthNames);

    public static Parser<char, int> RomanNumeral { get; }
        = Primitives.RomanNumeral(1, 12);
}
