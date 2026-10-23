using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers.Internals;
internal class YearParser
{
    private static int NormalizeYear(int year) => year < 40 ? year + 2000 : year + 1900;

    public static Parser<char, int> DigitOn2 { get; } = CreateDigitOn2(NormalizeYear);
    public static Parser<char, int> DigitOn4 { get; } = Primitives.FourDigitParser();
    public static Parser<char, int> RomanNumeral { get; }
        = Primitives.RomanNumber;
    public static Parser<char, int> RomanNumeralShort { get; }
        = CreateRomanNumeralShort(NormalizeYear);

    public static Parser<char, int> CreateDigitOn2(Func<int, int> normalizeYear)
        => Primitives.TwoDigitParser(normalizeYear);

    public static Parser<char, int> CreateRomanNumeralShort(Func<int, int> normalizeYear)
        => Primitives.RomanNumeral(0, 99, normalizeYear);
}

