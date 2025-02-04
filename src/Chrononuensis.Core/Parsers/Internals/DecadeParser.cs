using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;

namespace Chrononuensis.Parsers.Internals;
internal class DecadeParser
{
    private static Func<int, int> _normalizeDecade = (int decade) => decade < 40 ? decade + 2000 : decade + 1900;
    
    public static void NormalizeDecade(Func<int, int> normalizeDecade) => _normalizeDecade = normalizeDecade;
    public static Parser<char, int> DigitOn2 { get; } = Primitives.OneDigitThenZeroParser(_normalizeDecade);
    public static Parser<char, int> DigitOn4 { get; } = Primitives.ThreeDigitThenZeroParser();
}

