using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers.Internals;
internal class OlympiadParser
{
    public static Parser<char, int> Digit { get; } = Primitives.OneOrTwoDigitParser(1, 40);
    public static Parser<char, int> RomanNumeralShort { get; } = Primitives.RomanNumeral(1, 40);
}

