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
    private static Func<int, int> _normalizeYear = (int year) => year < 40 ? year + 2000 : year + 1900;
    
    public static void NormalizeYear(Func<int, int> normalizeYear) => _normalizeYear = normalizeYear;
    public static Parser<char, int> TwoDigit { get; } = Primitives.TwoDigitParser(_normalizeYear);
    public static Parser<char, int> FourDigit { get; } = Primitives.FourDigitParser();
}

